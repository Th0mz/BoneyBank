using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Boney
{
    class Boney {

        static void Main(string[] args) {
 

            BoneyState state = new BoneyState();

            // paxos frontend vai abrir conexões com todos os servers boney
            // ou seja primeiro deve ser comunicada a lista de servidores
            PaxosFrontend paxosClient = new PaxosFrontend();

            Server server = new Server
            {
                Services = { CompareAndSwapService.BindService(new CompareAndSwapImpl(state)), 
                             PaxosService.BindService(new PaxosImpl(state))},
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