using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boney
{

    public class PaxosFrontend {
        ServerState _serverState;

        private const int _OK = 1;
        private const int _NOK = -1; 
        // TODO : initialize proposal number

        public PaxosFrontend(ServerState serverState) {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            _serverState = serverState; 
        }

        public void propose(int slot, int leader) {
            // full paxos propose algoritm

            int last_round = 0;
            int id = _serverState.get_id();
            int number_servers = _serverState.get_paxos_servers().Count();

            int proposal_number = id;

            bool consensus_reached = false;
            bool quorum_reached = false;

            //TODO SUBSCRIBE FOR EVENT OF VALUE BEING ELECTED
            // TODO timeouts

            // TODO : dar clean do estado dos acceptors

            //TODO locks tbm aqui no frontend???

            while (!consensus_reached) {

                // Phase 1 : send prepares
                int highest_value = 0;
                int highest_sequence_number = 0;
                if (mustStop) break; //is it necessary here??
                while (!quorum_reached)
                {
                    highest_value = 0;
                    highest_sequence_number = 0;

                    proposal_number = (last_round + 1) * number_servers + id;
                    var proposal_replies = prepare(proposal_number, slot);

                    int count = 0;
                    // wait for replies
                    while (proposal_replies.Any())
                    {
                        var task_reply = Task.WhenAny(proposal_replies).Result;
                        var reply = task_reply.Result;

                        if (reply.CurrentInstance != true) return;

                        // check if the propose was promissed by the acceptor
                        if (reply.LastPromisedSeqnum == proposal_number) {
                            if (reply.LastAcceptedSeqnum > highest_sequence_number) {
                                highest_sequence_number = reply.LastAcceptedSeqnum;
                                highest_value = reply.LastAcceptedValue;
                                
                            }
                            count++;

                        } else {
                            last_round = reply.LastPromisedSeqnum / number_servers;
                        }

                        // remove recieved reply
                        proposal_replies.Remove(task_reply);
                    }
                    if (mustStop) break;

                    if (highest_sequence_number == 0) {
                        highest_value = leader;
                    }

                    // got quorum
                    if (count >= (number_servers / 2 ) + 1) {
                        quorum_reached = true;
                    }
                }

                // Phase 2 : send accepts
                quorum_reached = false;
                var accept_replies = accept(highest_sequence_number, highest_value, slot);
                
                int accept_count = 0;
                while (accept_replies.Any())
                {
                    var task_reply = Task.WhenAny(accept_replies).Result;
                    var reply = task_reply.Result;

                    if (reply.CurrentInstance != true) return;

                    if (reply.Status == _OK) {
                        accept_count++;
                    }

                    // remove recieved reply
                    accept_replies.Remove(task_reply);
                }

                // check if a majority replied
                if ((accept_count >= (number_servers / 2) + 1)) {
                    continue;
                }


                // Propagate consensus value
                learn(highest_value, slot);
            }

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
            foreach (var client in _serverState.get_paxos_servers().Values)
            {
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
            foreach (var client in _serverState.get_paxos_servers().Values) {
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

            foreach (var client in _serverState.get_paxos_servers().Values) {
                client.LearnAsync(request);
            }
        }




    }
}
