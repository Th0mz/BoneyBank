using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boney
{

    public class PaxosFrontend {
        // hardcoded max number of processes
        private static int max = 3;
        private Dictionary<int, PaxosService.PaxosServiceClient> boneyServers = 
            new Dictionary<int, PaxosService.PaxosServiceClient>();

        public PaxosFrontend () {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
        }

        public void add_server (int id, string url) {
            GrpcChannel channel = GrpcChannel.ForAddress(url);
            PaxosService.PaxosServiceClient client = new PaxosService.PaxosServiceClient(channel);

            boneyServers.Add(id, client);
        }

        public void prepare (int proposal_number) {
            PrepareRequest request = new PrepareRequest {
                ProposalNumber = proposal_number
            };

            // sequencial
            foreach (PaxosService.PaxosServiceClient client in boneyServers.Values) {
                client.Prepare(request);
            }
        }
        

    }
}
