using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using System.Diagnostics.Metrics;

namespace Boney
{

    public class PaxosFrontend {
        // TODO : use read/write locks??? locks that permit
        // multiple reads if there is no one writting
        ServerState _serverState;
        private bool initialized_connections = false;

        private int last_round = 0;
        private object mutex = new Object();

        public PaxosFrontend(ServerState serverState) {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            _serverState = serverState; 
        }

        public void setup_connections () {
            if (!initialized_connections) {
                foreach (PaxosServerConnection serverConnection in _serverState.get_paxos_servers().Values) {
                    serverConnection.setup_stub(_serverState);
                }

                initialized_connections = true;
            }
        }

        public void propose(int slot, int leader) {
            
            setup_connections();
            // full paxos propose algoritm

            int timeout_exp = 0;
            int id = _serverState.get_id();
            int number_servers = _serverState.get_paxos_servers().Count();

            int proposal_number = id;

            bool consensus_reached = false;
            bool quorum_reached = false;

            Random random = new Random();


            while (!consensus_reached) {

                // Phase 1 : send prepares
                int highest_value = 0;
                int highest_sequence_number = 0;

                while (!quorum_reached)
                {
                    highest_value = 0;
                    highest_sequence_number = 0;

                    lock (mutex)
                    {
                        proposal_number = last_round * number_servers + id;
                        last_round++;
                    }
                    timeout_exp++;

                    var proposal_replies = prepare(proposal_number, slot);

                    int count = 0;
                    // wait for replies or a quorum of acks
                    while (proposal_replies.Any() && count < (number_servers / 2) + 1)
                    {
                        var task_reply = Task.WhenAny(proposal_replies).Result;
                        var reply = task_reply.Result;

                        if (reply.CurrentInstance != true) {
                            return;
                        }

                        // check if the propose was promissed by the acceptor
                        if (reply.LastPromisedSeqnum == proposal_number) {
                            if (reply.LastAcceptedSeqnum > highest_sequence_number) {
                                highest_sequence_number = reply.LastAcceptedSeqnum;
                                highest_value = reply.LastAcceptedValue;
                                
                            }
                            count++;

                        } else {
                            lock (mutex)
                            {
                                last_round = reply.LastPromisedSeqnum / number_servers;
                            }
                        }

                        // remove recieved reply
                        proposal_replies.Remove(task_reply);
                    }


                    // quorum not reached (must do another prepare)
                    if (!(count >= (number_servers / 2 ) + 1)) {
                        // random timeout
                        int timeout = random.Next(0, (int)Math.Pow(2, timeout_exp + 1));
                        Thread.Sleep(timeout);
                        continue;
                    }

                    if (highest_sequence_number == 0) {
                        highest_value = leader;
                    }

                    quorum_reached = true;
                }


                // Phase 2 : send accepts
                quorum_reached = false;

                var accept_replies = accept(proposal_number, highest_value, slot);
                int accept_count = 0;
                // wait for replies or a quorum of acks
                while (accept_replies.Any() && accept_count < (number_servers / 2) + 1)
                {
                    var task_reply = Task.WhenAny(accept_replies).Result;
                    var reply = task_reply.Result;

                    if (reply.CurrentInstance != true) {
                        return;
                    }

                    if (reply.Status == ResponseCode.Ok) {
                        accept_count++;
                    }

                    // remove recieved reply
                    accept_replies.Remove(task_reply);
                }

                // check if a majority replied
                if (!(accept_count >= (number_servers / 2) + 1)) {
                    // random timeout
                    int timeout = random.Next(0, (int) Math.Pow(2, timeout_exp + 1));
                    Thread.Sleep(timeout);
                    continue;
                }

                // Propagate consensus value
                learn(highest_value, slot);
                consensus_reached = true;
                
            }

            return;
        }

        public List<Task<PrepareReply>> prepare (int proposal_number, int slot) {
            PrepareRequest request = new PrepareRequest {
                ProposalNumber = proposal_number,
                Slot = slot
            };

            List<Task<PrepareReply>> replies = new List<Task<PrepareReply>>();
            foreach (PaxosServerConnection connection in _serverState.get_paxos_servers().Values)
            {
                var client = connection.get_client();
                var reply = client.PrepareAsync(request).ResponseAsync;
                replies.Add(reply);
            }

            return replies;
        }

        public List<Task<AcceptReply>> accept (int proposal_number, int leader, int slot) {
            AcceptRequest request = new AcceptRequest
            {
                ProposalNumber = proposal_number,
                Leader = leader,
                Slot = slot
            };

            List<Task<AcceptReply>> replies = new List<Task<AcceptReply>>();
            foreach (PaxosServerConnection connection in _serverState.get_paxos_servers().Values) {
                var client = connection.get_client();
                var reply = client.AcceptAsync(request).ResponseAsync;
                replies.Add(reply);
            }

            return replies;
        }

        public void learn(int leader, int slot) {
            LearnRequest request = new LearnRequest {
                Leader = leader,
                Slot = slot
            };

            foreach (PaxosServerConnection connection in _serverState.get_paxos_servers().Values) {
                var client = connection.get_client();
                client.LearnAsync(request);
            }
        }
    }

    public class PaxosServerConnection {
        private string _url;
        private GrpcChannel _channel;
        private PaxosService.PaxosServiceClient _client;
        private FrozenInterceptor _interceptor;
        private bool setup = false;

        public PaxosServerConnection(string url) {
            this._url = url;
        }

        public void setup_stub(ServerState serverState) {
            if (!setup) {
                _interceptor = new FrozenInterceptor(serverState);
                _channel = GrpcChannel.ForAddress(_url);

                CallInvoker interceptingInvoker = _channel.Intercept(_interceptor);
                _client = new PaxosService.PaxosServiceClient(interceptingInvoker);

                setup = true;
            }
        }

        public void toggle_freeze () {

            if (_interceptor == null) {
                return;
            }

            _interceptor.toggle_freeze();
        }

        public PaxosService.PaxosServiceClient get_client() {
            return _client;
        }
    }
}
