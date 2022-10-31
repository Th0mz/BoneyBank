using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Grpc.Core;
using System.ComponentModel.Design;
using System.Threading;

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
                foreach (BankPaxosServerConnection serverConnection in _serverState.get_bank_servers().Values) {
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

       public void doCommand(CommandId commandId)
        {
            //create the grpc channels to other bank servers if they dont already exist
            setup_connections();
            
            //TODO: locks
            int id = _serverState.get_id();
            int sequence_number = _serverState.get_next_sequence_number();
            int assignment_slot = _serverState.get_current_slot();
            
            // TODO : sequence number é mesmo necessário?
            var tentative_replies = tentative(id, sequence_number, assignment_slot, commandId);
            int number_servers = _serverState.get_bank_servers().Count();
            int count = 0;
           
            while (tentative_replies.Any() && (count < (number_servers / 2) + 1))
            {
                var task_reply = Task.WhenAny(tentative_replies).Result;
                var reply = task_reply.Result;

                // check if the command was acked by the other banks
                if (reply.Ack) count++;

                // remove recieved reply
                tentative_replies.Remove(task_reply);
            }
            if(count >= (number_servers / 2) + 1) {
                commit(commandId, sequence_number);
            } else {
                //TODO: faz alguma coisa???
            }
        }

        public void doCleanup () {
            // TODO :  
        }

        public List<Task<TentativeReply>> tentative(int id, int sequence_number, int assignment_slot, CommandId commandId)
        {
            TentativeRequest request = new TentativeRequest
            {
                RequestId = commandId,
                SequenceNumber = sequence_number,
                AssignmentSlot = assignment_slot,
                SenderId = id

            };

            List<Task<TentativeReply>> replies = new List<Task<TentativeReply>>();
            foreach (BankPaxosServerConnection connection in _serverState.get_bank_servers().Values)
            {
                var client = connection.get_client();
                var reply = client.TentativeAsync(request).ResponseAsync; //TODO: should it be async???
                replies.Add(reply);
            }

            return replies;
        }

        public void commit(CommandId commandId, int sequence_number) {
            CommitRequest request = new CommitRequest
            {
                RequestId = commandId,
                SequenceNumber = sequence_number
            };

            foreach (BankPaxosServerConnection connection in _serverState.get_bank_servers().Values)
            {
                var client = connection.get_client();
                client.CommitAsync(request);
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
