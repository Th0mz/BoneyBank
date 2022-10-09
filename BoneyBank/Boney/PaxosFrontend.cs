using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boney
{

    public class PaxosFrontend {
        
        private BoneyState _state;
        public PaxosFrontend (BoneyState state) {
            _state = state;
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
        }

        public void add_server (int id, string url) { _state.add_server(id, url); }

        public void prepare (/*int proposal_number*/) {
            int myId = _state.getId();
            int lastSeqNumber = _state.getLastSeqNumber();
            int numberProcesses = _state.getNumberProcesses();
            int numberRound = lastSeqNumber / numberProcesses;
            int myProposalNumber =  (numberRound+1)*numberProcesses + myId
            PrepareRequest request = new PrepareRequest {
                ProposalNumber = myProposalNumber
            };

            Dictionary<int, PaxosService.PaxosServiceClient> _boneyServers = _state.getBoneyServers();
            PrepareReply reply1, reply2, reply3;
            Parallel.Invoke(
                () => reply1 = _boneyServers[1].Prepare(request),
                () => reply2 = _boneyServers[2].Prepare(request),
                () => reply3 = _boneyServers[3].Prepare(request)
                );

            //has to send an accept with the value of the previous highest proposal
            int leaderToChoose;
            if (reply1.proposal_number >= reply2.proposal_number)
            {
                if(reply1.proposal_number >= reply3.proposal_number)
                {
                    leaderToChoose = reply1.leader;
                } else
                {
                    leaderToChoose = reply3.leader;
                }
            } else
            {
                if (reply2.proposal_number >= reply3.proposal_number)
                {
                    leaderToChoose = reply2.leader;
                }
                else
                {
                    leaderToChoose = reply3.leader;
                }
            }
            AcceptRequest newRequest = new AcceptRequest { Leader = leaderToChoose, 
                                                           Proposal_number = myProposalNumber};
            AcceptReply r1, r2, r3;

            Parallel.Invoke(
                () => r1 = _boneyServers[1].Accept(newRequest),
                () => r2 = _boneyServers[2].Accept(newRequest),
                () => r3 = _boneyServers[3].Accept(newRequest)
                );
            int count = 0;
            if (r1 == 1) count++;
            if (r2 == 1) count++;
            if (r3 == 1) count++;
            if (count >= 2)
            {
                LearnRequest req = new LearnRequest { Leader = leaderToChoose };
                Parallel.Invoke(
                    () => _boneyServers[1].Learn(req),
                    () => _boneyServers[2].Learn(req),
                    () => _boneyServers[3].Learn(req)
                );
            }
            //foreach (PaxosService.PaxosServiceClient client in boneyServers.Values) {
            //    client.Prepare(request);
            //}
        }
        

    }
}
