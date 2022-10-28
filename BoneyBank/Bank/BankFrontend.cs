using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Grpc.Core;

namespace Bank
{
    public class BankFrontend {
        private ServerState _serverState;
        private bool initialized_connections = false;

        public BankFrontend (ServerState serverState) {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            _serverState = serverState; 
        }

        public void setup_connections() {
            if (!initialized_connections) {
                foreach (BankPaxosServerConnection serverConnection in _serverState.get_paxos_servers().Values) {
                    serverConnection.setup_stub();
                }
                initialized_connections = true;
            }
        }

        // this method shouldn't be async
        public void compareAndSwap (int slot, int leader) {
            CompareAndSwapRequest request = new CompareAndSwapRequest { Slot = slot, Leader = leader };

            List<Task<CompareAndSwapReply>> replies = new List<Task<CompareAndSwapReply>>(); 
            foreach (var client in _serverState.get_boney_servers().Values){
                var reply = client.CompareAndSwapAsync(request).ResponseAsync;
                replies.Add(reply);
            }

            while (replies.Any()) {

                var task_reply = Task.WhenAny(replies).Result;
                var reply = task_reply.Result;

                Console.WriteLine("received reply with leader " + reply.Leader);

                // remove recieved reply
                replies.Remove(task_reply);
            }
        }

        public void doCommand(string commandId)
        {
            //create the grpc channels to other bank servers if they dont already exist
            setup_connections();

            if (_serverState.is_coordinator()) {
                //TODO:
            }

        }
    }



    public class BankPaxosServerConnection
    {
        private string _url;
        private GrpcChannel _channel;
        private BankPaxos.BankPaxosClient _client;

        public BankPaxosServerConnection(string url) {
            this._url = url;
        }

        public void setup_stub() {
            _channel = GrpcChannel.ForAddress(_url);
            _client = new BankPaxos.BankPaxosClient(_channel);
        }

        public BankPaxos.BankPaxosClient get_client() {
            return _client;
        }
    }
}
