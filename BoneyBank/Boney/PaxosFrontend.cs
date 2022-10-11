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

        public PaxosFrontend(ServerState serverState) {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            _serverState = serverState; 
        }

        public void prepare (int proposal_number) {
            PrepareRequest request = new PrepareRequest {
                ProposalNumber = proposal_number
            };

            // sequencial
            //foreach (PaxosService.PaxosServiceClient client in boneyServers.Values) {
            //    client.Prepare(request);
            //}
        }
        

    }
}
