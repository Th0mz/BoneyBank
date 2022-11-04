using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class ClientFrontend
    {
        bool initialized_connections = false;
        private ClientState _clientState;
        public ClientFrontend(ClientState state) {
            _clientState = state;
        }

        public void setup_connections() {
            if (!initialized_connections) {
                foreach (var serverConnection in _clientState.get_bank_servers().Values) {
                    serverConnection.setup_stub();
                }

                initialized_connections = true;
            }
        }

        public requestId get_request_id () {
            return new requestId {
                ClientId = _clientState.get_id(),
                ClientSequenceNumber = _clientState.get_sequence_number(),
            };
        }

        public void deposit (float amount) {
            setup_connections();

            var id = get_request_id();
            var request = new DepositRequest { Amount = amount, Id = id};

            List<Task<DepositReply>> replies = new List<Task<DepositReply>>();
            foreach (var bankConnection in _clientState.get_bank_servers().Values) {
                var client = bankConnection.get_client();
                var reply = client.DepositAsync(request).ResponseAsync;

                replies.Add(reply);
            }

            // wait just for one reply
            var task_reply = Task.WhenAny(replies).Result;
            var _reply = task_reply.Result;

            Console.WriteLine("[" + _reply.Server + "]" + " Deposit succesful");
            replies.Remove(task_reply);
        }

        public void withdrawal(float amount) {
            setup_connections();


            var id = get_request_id();
            var request = new WithdrawalRequest { Amount = amount, Id = id };
            List<Task<WithdrawalReply>> replies = new List<Task<WithdrawalReply>>();
            foreach (var bankConnection in _clientState.get_bank_servers().Values) {
                var client = bankConnection.get_client();
                var reply = client.WithdrawalAsync(request).ResponseAsync;


                replies.Add(reply);
            }

            var task_reply = Task.WhenAny(replies).Result;
            var _reply = task_reply.Result;

            Console.WriteLine("[" + _reply.Server + "]" + " Withdrawal " + _reply.Status);
            replies.Remove(task_reply);
        }

        public void readBalance () {
            setup_connections();

            var id = get_request_id();
            var request = new ReadBalanceRequest{ Id = id };
            List<Task<ReadBalanceReply>> replies = new List<Task<ReadBalanceReply>>();
            foreach (var bankConnection in _clientState.get_bank_servers().Values)
            {
                var client = bankConnection.get_client();
                var reply = client.ReadBalanceAsync(request).ResponseAsync;

                replies.Add(reply);
            }

            // wait just for one reply
            var task_reply = Task.WhenAny(replies).Result;
            var _reply = task_reply.Result;

            Console.WriteLine("[" + _reply.Server + "]" + "Balance : " + _reply.Balance);
            replies.Remove(task_reply);

        }
    }

    public class BankServerConnection
    {
        private string _url;
        private GrpcChannel _channel;
        private BankService.BankServiceClient _client;

        public BankServerConnection(string url) {
            _url = url;
        }

        public void setup_stub() {
            _channel = GrpcChannel.ForAddress(_url);
            _client = new BankService.BankServiceClient(_channel);
        }

        public BankService.BankServiceClient get_client() {
            return _client;
        }
    }
}
