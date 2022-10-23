using Grpc.Core.Interceptors;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Bank
{
    public class ServerInfo {
        private int _id;
        private bool _frozen;
        private bool _suspected;

        public ServerInfo(int id, bool frozen, bool suspected) {
            _id = id;
            _frozen = frozen;
            _suspected = suspected;
        }
    }
    public class ServerState {

        // constants
        private const string _FROZEN = "F";
        private const string _SUSPECTED = "S";

        // this process state
        private int _id;
        private string _url = "";
        private TimeSpan _delta;
        static private bool _frozen;
        private DateTime _starting_time;
        static public ManualResetEvent _event;

        // F list

        // other servers info
        // instead of the url store the grpc stub <<<
        private Dictionary<int, string> _banks = new Dictionary<int, string>();
        private Dictionary<int, CompareAndSwapService.CompareAndSwapServiceClient> _bonies = new Dictionary<int, CompareAndSwapService.CompareAndSwapServiceClient>();
        
        private int _current_slot = 0;
        private Dictionary<int, ServerInfo[]> _timeslots_info = new Dictionary<int, ServerInfo[]>();


        public ServerState()
        {
            _event = new ManualResetEvent(false);
        }

        public Dictionary<int, CompareAndSwapService.CompareAndSwapServiceClient> get_boney_servers () {
            return _bonies;
        }

        public int get_id() {
            return _id;
        }

        public TimeSpan get_delta () {
            return _delta;
        }

        public bool has_next_slot () {
            return _current_slot < _timeslots_info.Count;
        }

        public DateTime get_starting_time() {
            return _starting_time;
        }

        public int get_current_slot () {
            return _current_slot;
        }

        public bool add_server (string sid, string _class, string url) {
            // convert id
            int id;
            if (!int.TryParse(sid, out id)) {
                return false;
            }

            // add this server info
            if (id == _id) {
                _url = url;
                return true;
            }

            // add other server info
            bool added_server = false;
            if (_class.Equals("bank")) {

                _banks.Add(id, url);
                added_server = true;
            } else if (_class.Equals("boney")) {

                var clientInterceptor = new ClientInterceptor();

                GrpcChannel channel = GrpcChannel.ForAddress(url);
                CallInvoker interceptingInvoker = channel.Intercept(clientInterceptor);

                CompareAndSwapService.CompareAndSwapServiceClient client =
                    new CompareAndSwapService.CompareAndSwapServiceClient(interceptingInvoker);


                _bonies.Add(id, client);
                added_server = true;
            }

            return added_server;
        }

        public bool set_id(int id) {
            _id = id;
            return true;
        }

        public bool set_starting_date(string starting_date) {
            _starting_time = Convert.ToDateTime(starting_date);
            
            // TODO : check ToDateTime error
            return true;
        }

        public bool set_delta (string delta) {

            int _delta_int;
            if (!int.TryParse(delta, out _delta_int)) {
                return false;
            }

            _delta = TimeSpan.FromMilliseconds(_delta_int);
            return true;
        }

        public bool add_timeslot(string slot, string[] timeslot_info) {

            int _slot;
            if (!int.TryParse(slot, out _slot)) {
                return false;
            }


            int i = 0;
            int server_number = timeslot_info.Length;
            ServerInfo[] servers_info = new ServerInfo[server_number];

            foreach (string info in timeslot_info) {
                string[] info_splited = info.Split(')')[0].Split(", ");

                string id = info_splited[0];
                string frozen = info_splited[1];
                string suspected = info_splited[2];

                // conver values
                int server_id;
                if (!int.TryParse(id, out server_id)) {
                    return false;
                }

                bool server_frozen = frozen.Equals(_FROZEN);
                _frozen = server_frozen;
                bool server_suspected = suspected.Equals(_SUSPECTED);


                if (!_frozen)
                {
                    _event.Set();
                }
                else
                {
                    _event.Reset();
                }


                // add new entry
                ServerInfo server_info = new ServerInfo(server_id, server_frozen, server_suspected);
                servers_info[i] = server_info;
                i++;

            }

            _timeslots_info.Add(_slot, servers_info);
            return true;
        }

        public void setup_timeslot () {

            _current_slot++;
            // TODO : setup fronzen and current_slot

            return;
        }


        public class ClientInterceptor : Interceptor
        {

            public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
            {


                //set para dar unfreeze, senao vai reset
                //depois falta codigo para mudar o estado freeze e unfreeze

                if (ServerState._frozen)
                {
                    ServerState._event.WaitOne();
                }

                //xixkebeb quanto ta frozen mete a dormir
                AsyncUnaryCall<TResponse> response = base.AsyncUnaryCall(request, context, continuation);
                return response;
            }
        }
    }
}
