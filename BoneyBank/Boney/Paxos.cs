using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boney
{

    /*---------------------------------------------------------------------*/
    /*------------------------------PROPOSER-------------------------------*/
    /*---------------------------------------------------------------------*/
    public class Proposer
    {
        private Boney _boney;
        private Dictionary<int, PaxosService.PaxosServiceClient> _boneyServers =
            new Dictionary<int, PaxosService.PaxosServiceClient>();
        public Proposer(Boney boney) { 
            _boney = boney;
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
        }

        public void add_server(int id, string url)
        {
            GrpcChannel channel = GrpcChannel.ForAddress(url);
            PaxosService.PaxosServiceClient client = new PaxosService.PaxosServiceClient(channel);

            _boneyServers.Add(id, client);
        }

        public void runPaxosInstance(int slot, int leader)
        {
            PrepareRequest request = new PrepareRequest
            {
                ProposalNumber = proposal_number
            };
            PrepareResponse presp1, presp2, presp3;
            Parallel.Invoke(
                () => presp1 = _boneyServers[1].Prepare(request),
                () => presp2 = _boneyServers[2].Prepare(request),
                () => presp3 = _boneyServers[3].Prepare(request)
            );
            
        }
    }



    /*---------------------------------------------------------------------*/
    /*------------------------------ACCEPTOR-------------------------------*/
    /*---------------------------------------------------------------------*/
    public class Acceptor
    {
        private Boney _boney;
        public Acceptor(Boney boney) { _boney = boney; }
    }




    /*---------------------------------------------------------------------*/
    /*-------------------------------LEARNER-------------------------------*/
    /*---------------------------------------------------------------------*/
    public class Learner
    {
        private Boney _boney;
        public Learner(Boney boney) { _boney = boney; }
    }
}
