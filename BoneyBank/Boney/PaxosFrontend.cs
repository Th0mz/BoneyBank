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
        
        // TODO : initialize proposal number
        int proposalNumber = 0;

        public PaxosFrontend(ServerState serverState) {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            _serverState = serverState; 
        }

        public void propose (int slot, int leader) {
            // full paxos propose algoritm
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
        }

        public void prepare (int proposal_number) {
            PrepareRequest request = new PrepareRequest {
                ProposalNumber = proposal_number
            };

            List<Task<PrepareReply>> replies = new List<Task<PrepareReply>>();
            foreach (var client in _serverState.get_paxos_servers().Values)
            {
                var reply = client.PrepareAsync(request).ResponseAsync;
                replies.Add(reply);
            }
            //retornar replies 

            // deve ser feito na função principal
            while (replies.Any())
            {
                var task_reply = Task.WhenAny(replies).Result;
                var reply = task_reply.Result;


                // remove recieved reply
                replies.Remove(task_reply);
            }
        }

        public void accept (int proposal_number, int leader) {
            AcceptRequest request = new AcceptRequest
            {
                ProposalNumber = proposal_number,
                Leader = leader
            };

            List<Task<AcceptReply>> replies = new List<Task<AcceptReply>>();
            foreach (var client in _serverState.get_paxos_servers().Values) {
                var reply = client.AcceptAsync(request).ResponseAsync;
                replies.Add(reply);
            }
            //retornar replies 

            // deve ser feito na função principal
            while (replies.Any())
            {
                var task_reply = Task.WhenAny(replies).Result;
                var reply = task_reply.Result;


                // remove recieved reply
                replies.Remove(task_reply);
            }
        }

        public void learn(int leader) {
            LearnRequest request = new LearnRequest {
                Leader = leader
            };

            foreach (var client in _serverState.get_paxos_servers().Values) {
                client.LearnAsync(request);
            }
        }




    }
}
