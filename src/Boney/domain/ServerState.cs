using Grpc.Core.Interceptors;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static Grpc.Core.Interceptors.Interceptor;
using System.Drawing;

namespace Boney
{
    public class PaxosSlot {
        private int lastAcceptedValue = 0;
        private int lastAcceptedSeqnum = 0;
        private int lastPromisedSeqnum = 0;

        public int LastAcceptedValue {
            get { return lastAcceptedValue; }
            set { lastAcceptedValue = value; }
        }

        public int LastAcceptedSeqnum {
            get { return lastAcceptedSeqnum; }
            set { lastAcceptedSeqnum = value; }
        }
        public int LastPromisedSeqnum {
            get { return lastPromisedSeqnum; }
            set { lastPromisedSeqnum = value; }
        }

    }

    public class ServerState {

        // constants
        private const string _FROZEN = "F";
        private const string _SUSPECTED = "S";
        private const string _BONEY = "boney";
        private const string _BANK = "bank";

        // this process state
        private int _id;
        private string _url = "";
        private TimeSpan _delta;
        private bool _frozen;
        private int _coordinator;
        public Object _coordinatorLock = new();
        private DateTime _starting_time;

        // F list

        // other servers info
        // instead of the url store the grpc stub <<<
        private Dictionary<int, PaxosServerConnection> _bonies = new Dictionary<int, PaxosServerConnection>();

        private int _max_slots = 0;
        private int _current_slot = 0;
        private Dictionary<int, ServerInfo[]> _timeslots_info = new Dictionary<int, ServerInfo[]>();

        private List<FrozenInterceptor> _server_interceptors = new List<FrozenInterceptor>();

        private int _size;
        private PaxosSlot[] slotValues;

        public ServerState() {}

        public void initialize() {
            _size = _max_slots + 1;
            slotValues = new PaxosSlot[_size];

            for (int i = 0; i < _size; i++)
            {
                slotValues[i] = new PaxosSlot();
            }
        }


        // getters
        public PaxosSlot getPaxosSlot(int slot) { return slotValues[slot]; }
        public int getLastAcceptedValue (int slot) { return slotValues[slot].LastAcceptedValue; }
        public int getLastAcceptedSeqnum(int slot) { return slotValues[slot].LastAcceptedSeqnum; }
        public int getLastPromisedSeqnum(int slot) { return slotValues[slot].LastPromisedSeqnum; }

        public void setLastAcceptedValue (int slot, int newvalue) { slotValues[slot].LastAcceptedValue = newvalue; }
        public void setLastAcceptedSeqnum(int slot, int newvalue) { slotValues[slot].LastAcceptedSeqnum = newvalue; }
        public void setLastPromisedSeqnum(int slot, int newvalue) { slotValues[slot].LastPromisedSeqnum = newvalue; }

        public bool is_frozen () {
            return _frozen;
        }

        public bool is_coordinator() {
            lock(_coordinatorLock) {
                return _coordinator == _id;
            }
        }

        public int get_coordinator_id() {
            lock (_coordinatorLock) {
                return _coordinator;
            }
        }

        public string get_url() {
            return _url;
        }

        public DateTime get_starting_time () {
            return _starting_time;
        }

        public int get_id() {
            return _id;
        }

        public TimeSpan get_delta() {
            return _delta;
        }

        public bool has_next_slot() {
            return _current_slot < _max_slots;
        }

        public int get_current_slot() {
            return _current_slot;
        }

        public int get_max_slot () {
            return _max_slots;
        }

        public Dictionary<int, PaxosServerConnection> get_paxos_servers() {
            return _bonies;
        }

        public bool set_id(int id) {
            _id = id;
            return true;
        }

        public bool set_starting_time(string starting_date) {
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

        public void add_server_interceptor(FrozenInterceptor interceptor) {
            _server_interceptors.Add(interceptor);
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
                bool server_suspected = suspected.Equals(_SUSPECTED);

                // check server type
                string type = _BANK;
                if (_bonies.ContainsKey(server_id)) {
                    type = _BONEY;
                }

                // add new entry
                ServerInfo server_info = new ServerInfo(server_id, server_frozen, server_suspected, type);
                servers_info[i] = server_info;
                i++;

            }

            _timeslots_info.Add(_slot, servers_info);
            return true;
        }

        public bool add_server(string sid, string _class, string url) {
            // convert id
            int id;
            if (!int.TryParse(sid, out id)) {
                return false;
            }

            // add this server info
            if (id == _id) {
                _url = url;
            }

            // add other server info
            bool added_server = false;
            if (_class.Equals(_BONEY)) {

                PaxosServerConnection client = new PaxosServerConnection(url);

                _bonies.Add(id, client);
                added_server = true;
            }

            return added_server;
        }

        public bool set_max_slots (string max_slots) {
            int int_max_slots;
            if (!int.TryParse(max_slots, out int_max_slots)) {
                return false;
            }
            _max_slots = int_max_slots;
            return true;
        }

        public void setup_timeslot () {

            // TODO : need locks
            _current_slot++;

            var current_slot = _timeslots_info[_current_slot];
            bool was_frozen = _frozen;
            int coordinator = int.MaxValue;
            foreach (var server_info in current_slot)
            {
                int id = server_info.get_id();
                // check if the current process is frozen
                if (id == _id) {
                    _frozen = server_info.is_frozen();
                    // if not frozen and it has the smallest id, it must be the coordinator
                    if (!_frozen && coordinator > id) {
                        coordinator = id;
                    }

                    continue;
                }

                // check if the bank processess is the coordinator or not
                // coordinator is the smallest process id not suspected
                if (server_info.is_boney() && !server_info.is_suspected() && coordinator > id) {
                    coordinator = id;
                }
            }

            lock (_coordinatorLock) {
                _coordinator = coordinator;
            }
            // freeze/unfreeze channels
            if (was_frozen != _frozen) {
                // freeze/unfreeze client channels
                foreach (var server_connection in _bonies.Values) {
                    server_connection.toggle_freeze();
                }

                // freeze/unfreeze server channels
                foreach (var interceptor in _server_interceptors) {
                    interceptor.toggle_freeze();
                }
            }
        }
    }

    public class ServerInfo
    {
        private int _id;
        private bool _frozen;
        private bool _suspected;
        private string _type;

        public ServerInfo(int id, bool frozen, bool suspected, string type) {
            _id = id;
            _frozen = frozen;
            _suspected = suspected;
            _type = type;
        }

        public bool is_boney() {
            return _type.Equals("boney");
        }

        public int get_id() {
            return _id;
        }

        public bool is_frozen() {
            return _frozen;
        }

        public bool is_suspected() {
            return _suspected;
        }
    }
}
