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
        private GrpcChannel channel;
        private CompareAndSwapService.CompareAndSwapServiceClient client;

        public BankFrontend () {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            channel = GrpcChannel.ForAddress("http://localhost:5001");
            client = new CompareAndSwapService.CompareAndSwapServiceClient(channel);
        }

        public void compareAndSwap (int slot, int leader) {
            CompareAndSwapRequest request = new CompareAndSwapRequest { Slot = slot, Leader = leader };
            var reply = client.CompareAndSwap(request);

            Console.WriteLine(reply.Leader);

            return;
        }
    }
}
