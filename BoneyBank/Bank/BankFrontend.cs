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

        // this method shouldn't be async
        public void compareAndSwap (int slot, int leader) {
            CompareAndSwapRequest request = new CompareAndSwapRequest { Slot = slot, Leader = leader };

            List<Task<CompareAndSwapReply>> replies = new List<Task<CompareAndSwapReply>>(); 
            foreach (var client in _serverState.get_boney_servers().Values){
                var reply = client.CompareAndSwapAsync(request).ResponseAsync;
                replies.Add(reply);
            }

            while (replies.Any()) {
                var task_reply = Task.WhenAny(replies).Result;
                var reply = task_reply.Result;

                Console.WriteLine("received reply with leader " + reply.Leader);

                // remove recieved reply
                replies.Remove(task_reply);
            }
        }
    }
}
