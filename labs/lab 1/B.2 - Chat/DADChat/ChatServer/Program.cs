using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace chatServer {
    // ChatServerService is the namespace defined in the protobuf
    // ChatServerServiceBase is the generated base implementation of the service

    class Program {
        const int Port = 1001;
        static void Main(string[] args) {
            Server server = new Server
            {
                Services = { ChatServerService.BindService(new ServerService()) },
                Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
            };
            server.Start();
            Console.WriteLine("ChatServer server listening on port " + Port);
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();

            server.ShutdownAsync().Wait();

        }
    }
}

