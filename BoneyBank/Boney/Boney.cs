using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Boney
{
    class Boney {

        static void Main(string[] args) {
            BoneyState state = new BoneyState();
            Server server = new Server
            {
                Services = { CompareAndSwapService.BindService(new CompareAndSwapImpl())},
                Ports = { new ServerPort("localhost", 5001, ServerCredentials.Insecure) }
            };

            server.Start();

            Console.WriteLine("ChatServer server listening on port " + 5001);
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();

            server.ShutdownAsync().Wait();
        }
    }
}