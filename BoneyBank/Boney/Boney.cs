using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Boney
{
    class Boney {

        private static bool processInput(string[] args, ServerState serverState)
        {
            if (args.Length != 2) {
                Console.WriteLine("Error : invalid number of arguments");
                return false;
            }

            string path = args[0];
            if (!File.Exists(path)) {
                Console.WriteLine("Error : invalid path");
                return false;
            }

            int id;
            if (!int.TryParse(args[1], out id)) {
                Console.WriteLine("Error : process id is not a number");
                return false;
            }

            // read file 
            serverState.set_id(id);
            foreach (string line in File.ReadAllLines(path))
            {
                string[] parts = line.Split(' ');

                string command = parts[0];

                //TODO : do argument verifications ??
                switch (command)
                {
                    case "P":
                        if (parts[2].Equals("client") || parts[2].Equals("bank")) { continue; }

                        // TODO : check errors
                        serverState.add_server(parts[1], parts[2], parts[3]);
                        break;
                    case "T":
                        serverState.set_starting_date(parts[1]);
                        break;
                    case "S":
                        break;
                    case "D":
                        serverState.set_delta(parts[1]);
                        break;
                    case "F":
                        string slot = parts[1];
                        string[] timeslot_info = line.Split("(");

                        serverState.add_timeslot(slot, timeslot_info[1..]);
                        break;
                }
            }

            return true;
        }

        static void Main(string[] args) {
 

            BoneyState state = new BoneyState();
            ServerState serverState = new ServerState();

            if (!processInput(args, serverState)) {
                // error processing input occurred
                return;
            }

            // paxos frontend vai abrir conexões com todos os servers boney
            // ou seja primeiro deve ser comunicada a lista de servidores
            //PaxosFrontend paxosClient = new PaxosFrontend();

            string url = serverState.get_url();
            string[] urlSplit = url.Split(':');

            string host = urlSplit[1][2..];
            string _port = urlSplit[2];

            int port;
            if (!int.TryParse(_port, out port)) {
                Console.WriteLine("Error : Invalid port");
                return;
            }

            Server server = new Server
            {
                Services = { CompareAndSwapService.BindService(new CompareAndSwapImpl(state)), 
                             PaxosService.BindService(new PaxosImpl(state))},
                //meter aqui o url
                Ports = { new ServerPort(host, port, ServerCredentials.Insecure) }
            };

            server.Start();

            Console.WriteLine("ChatServer server listening on port " + port);
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();

            server.ShutdownAsync().Wait();
        }
    }
}