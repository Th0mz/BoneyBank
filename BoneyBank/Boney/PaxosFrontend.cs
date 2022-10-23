using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boney
{

    public class PaxosServerConnection {
        private string _url;
        private Interceptor _interceptor;
        private GrpcChannel _channel;
        private PaxosService.PaxosServiceClient _client;

        public PaxosServerConnection(string url, Interceptor interceptor) {
            this._url = url;
            this._interceptor = interceptor;
        }

        public void setup_stub() {
            _channel = GrpcChannel.ForAddress(_url);
            CallInvoker interceptingInvoker = _channel.Intercept(_interceptor);
            _client = new PaxosService.PaxosServiceClient(interceptingInvoker);
        }

        public PaxosService.PaxosServiceClient get_client() {
            return _client;
        }
    }

    public class PaxosFrontend {
        ServerState _serverState;
        private bool initialized_connections = false;

        private int last_round = 0;
        private object mutex = new Object();

        private const int _OK = 1;
        private const int _NOK = -1; 
        // TODO : initialize proposal number

        public PaxosFrontend(ServerState serverState) {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            _serverState = serverState; 
        }

        public void setup_connections () {
            if (!initialized_connections) {
                foreach (PaxosServerConnection serverConnection in _serverState.get_paxos_servers().Values) {
                    serverConnection.setup_stub();
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

            //TODO SUBSCRIBE FOR EVENT OF VALUE BEING ELECTED
            // TODO timeouts

            //TODO locks tbm aqui no frontend???

            // Console.WriteLine("[" + DateTime.Now.ToString("s.ffff") + "] " + "Proposer : starting propose of " + slot + ", " + leader);
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
                    // Console.WriteLine("[" + DateTime.Now.ToString("s.ffff") + "] " + "Proposer : sent prepare request with number " + proposal_number);

                    int count = 0;
                    // wait for replies
                    while (proposal_replies.Any())
                    {
                        var task_reply = Task.WhenAny(proposal_replies).Result;
                        var reply = task_reply.Result;

                        if (reply.CurrentInstance != true) {
                            // Console.WriteLine("[" + DateTime.Now.ToString("s.ffff") + "] " + "Proposer : not current instance");
                            return;
                        }

                        // Console.WriteLine("[" + DateTime.Now.ToString("s.ffff") + "] " + "Proposer : recived -  accepted_sn : "  + reply.LastAcceptedSeqnum 
                        //    + " promised_sn  " + reply.LastPromisedSeqnum + "  accepted_v " + reply.LastAcceptedValue);

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
                        // Console.WriteLine("[" + DateTime.Now.ToString("s.ffff") + "] " + "Proposer : quorum not reached");
                        int timeout = random.Next(0, (int)Math.Pow(2, timeout_exp + 1));
                        Thread.Sleep(timeout);
                        continue;
                    }

                    if (highest_sequence_number == 0) {
                        highest_value = leader;
                    }

                    // Console.WriteLine("[" + DateTime.Now.ToString("s.ffff") + "] " + "Proposer : after all replies " + highest_sequence_number + ", " + highest_value);
                    // Console.WriteLine("[" + DateTime.Now.ToString("s.ffff") + "] " + "Proposer : quorum reached");
                    quorum_reached = true;
                }


                // Phase 2 : send accepts
                quorum_reached = false;
                // Console.WriteLine("[" + DateTime.Now.ToString("s.ffff") + "] " + "Proposer : sent accept request with number " + proposal_number 
                //    + " and leader " + highest_value);

                var accept_replies = accept(proposal_number, highest_value, slot);
                
                int accept_count = 0;
                while (accept_replies.Any())
                {
                    var task_reply = Task.WhenAny(accept_replies).Result;
                    var reply = task_reply.Result;

                    if (reply.CurrentInstance != true) {
                        // Console.WriteLine("[" + DateTime.Now.ToString("s.ffff") + "] " + "Proposer : not current instance");
                        return;
                    }

                    // Console.WriteLine("[" + DateTime.Now.ToString("s.ffff") + "] " + "Proposer : recived accept status " + reply.Status);
                    if (reply.Status == _OK) {
                        accept_count++;
                    }

                    // remove recieved reply
                    accept_replies.Remove(task_reply);
                }

                // check if a majority replied
                if (!(accept_count >= (number_servers / 2) + 1)) {
                    // Console.WriteLine("[" + DateTime.Now.ToString("s.ffff") + "] " + "Proposer : quorum not reached");
                    // random timeout
                    int timeout = random.Next(0, (int) Math.Pow(2, timeout_exp + 1));
                    Thread.Sleep(timeout);
                    continue;
                }

                // Console.WriteLine("[" + DateTime.Now.ToString("s.ffff") + "] " + "Proposer : quorum reached");

                // Propagate consensus value
                learn(highest_value, slot);
                consensus_reached = true;
                Console.WriteLine("Proposed : learn " + highest_value);
                
            }

            return;
        }
                //HAS REACHED MAJORITY OF PROMISES
                //TODO SEND ACCEPT'S AND ACT ACCORDINGLY


                /*
                 var replies = prepare(proposal_number)
                 values = []
                 while {
                    var reply = WhenAny(replies);

                    if (values.status = abort)
                        fail

                    values.add(reply.value)

                    if (recieved a majority of replies)
                        replies.deleteAll()
                        continue
                 }

                  int leader = choose_leader(values)
                  var replies = accept(proposal_number, leader)
                  ok_count = 0
                  while {
                    var reply = WhenAny(replies);
                    if reply.status = ok { ok_count++; }
                 }

                  if (!ok_count >= majority)
                        fail

                  learn(leader)
                 */


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
}
