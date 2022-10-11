using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Grpc.Core;

namespace Bank
{
    internal class BankFrontend {
        private ServerState _serverState;

        public BankFrontend (ServerState serverState) {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            _serverState = serverState; 
        }

        public void compareAndSwap (int slot, int leader) {
            CompareAndSwapRequest request = new CompareAndSwapRequest { Slot = slot, Leader = leader };
            
            foreach (CompareAndSwapService.CompareAndSwapServiceClient client in _serverState.get_bonies().Values) {
                var reply = client.CompareAndSwap(request);
                
            }
        }
    }
}
