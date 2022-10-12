using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boney
{
    public class Paxos
    {
        private ServerState _serverState;
        private Dictionary<int, PaxosService.PaxosServiceClient> _boneyServers =
            new Dictionary<int, PaxosService.PaxosServiceClient>();

        public Paxos(ServerState serverState)
        {
            _serverState = serverState;            
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
        }

        public void add_server(int id, string url)
        {
            GrpcChannel channel = GrpcChannel.ForAddress(url);
            PaxosService.PaxosServiceClient client = new PaxosService.PaxosServiceClient(channel);

            _boneyServers.Add(id, client);
        }

        /*---------------------------------------------------------------------*/
        /*------------------------------PROPOSER-------------------------------*/
        /*---------------------------------------------------------------------*/
        public void runPaxosInstance(int slot, int leader)
        {
            int thisId = getId();
            int lastRound = _serverState.getLastRound();
            int numberServers = _serverState.getNumberServers();
            if(!_boneyServers) _boneyServers = _serverState.getBoneyServers();

            //TODO SUBSCRIBE FOR EVENT OF VALUE BEING ELECTED

            bool consensusReached = false;
            while (!consensusReached) {
                int preparedValue;
                bool quorumReached = false;
                while (!quorumReached) {
                    //choose next ballot number(seq_number)
                    int nextBallot = (lastRound + 1) * numberServers + thisId;
                    //send NextBallot(seq_number) to all servers known (fixed cell of 3 for now)
                    PrepareRequest request = new PrepareRequest
                    {
                        ProposalNumber = nextBallot
                    };
                    PrepareResponse presp1, presp2, presp3;
                    Parallel.Invoke(
                        () => presp1 = _boneyServers[1].Prepare(request),
                        () => presp2 = _boneyServers[2].Prepare(request),
                        () => presp3 = _boneyServers[3].Prepare(request)
                    ); //TODO DOES IT AWAIT FOR ALL TO FINISH???
                    int count = 0
                    int highestSeqNumber = 0;
                    foreach (int i in { presp1, presp2, presp3}){
                        if (i.Proposal_number == nextBallot) {
                            if (i.Last_proposal != null & i.Last_proposal > highestSeqNumber)
                            {
                                highestSeqNumber = i.Last_proposal;
                                preparedValue = i.Last_Proposal; //latest vote made by replica in quorum
                            }
                            count++;
                        }
                        if (i.Proposal_number > nextBallot)
                        {
                            count = 0;
                            break;
                        }

                    }
                    if (count >= 2 && highestSeqNumber == nextBallot) quorumReached = true;
                    _serverState.setLastRound(highestSeqNumber / numberServers);
                }

                //HAS REACHED MAJORITY OF PROMISES
                //TODO SEND ACCEPT'S AND ACT ACCORDINGLY

            }
        }
    }
}