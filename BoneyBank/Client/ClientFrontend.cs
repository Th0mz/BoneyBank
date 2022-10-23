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
            var request = new DepositRequest { Amount = amount, Id = id };
            foreach (var bankConnection in _clientState.get_bank_servers().Values) {
                var client = bankConnection.get_client();
                var reply = client.Deposit(request);

                // TODO : print out all replies received and the information of whether each
                // server is replying as a primary or a backup server.
                Console.WriteLine("Reply status : " + reply.Status);
            }

        }

        public void withdrawal(float amount) {
            setup_connections();

            var id = get_request_id();
            var request = new WithdrawalRequest { Amount = amount, Id = id };
            foreach (var bankConnection in _clientState.get_bank_servers().Values) {
                var client = bankConnection.get_client();
                var reply = client.Withdrawal(request);

                // TODO : print out all replies received and the information of whether each
                // server is replying as a primary or a backup server.
                Console.WriteLine("Reply status : " + reply.Status);
            }
        }

        public void readBalance () {
            setup_connections();

            var id = get_request_id();
            var request = new ReadBalanceRequest{ Id = id };
            foreach (var bankConnection in _clientState.get_bank_servers().Values)
            {
                var client = bankConnection.get_client();
                var reply = client.ReadBalance(request);

                // TODO : print out all replies received and the information of whether each
                // server is replying as a primary or a backup server.
                Console.WriteLine("Balance : " + reply.Balance);
            }
        }
    }

    public class BankServerConnection
    {
        private string _url;
        private GrpcChannel _channel;
        private BankService.BankServiceClient _client;

        public BankServerConnection(string url) {
            this._url = url;
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
