using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Boney
{
    class Boney {

        private static bool processInput(string[] args, string path, ServerState serverState)
        {
            if (args.Length != 1) {
                Console.WriteLine("Error : invalid number of arguments");
                return false;
            }

            if (!File.Exists(path)) {
                Console.WriteLine("Error : invalid path");
                return false;
            }

            int id;
            if (!int.TryParse(args[0], out id)) {
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
                        serverState.set_starting_time(parts[1]);
                        break;
                    case "S":
                        serverState.set_max_slots(parts[1]);
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

        static async Task Main(string[] args) {

            DateTime start = DateTime.Now;

            ServerState serverState = new ServerState();
            PaxosFrontend paxosFrontend = new PaxosFrontend(serverState);
            //string config_path = @"..\..\..\..\..\configuration_sample.txt";
            string config_path = @"C:\Users\tomas\OneDrive\Ambiente de Trabalho\Uni\4Ano\P1\PADI\projeto\configuration_sample.txt";


            if (!processInput(args, config_path, serverState)) {
                // error processing input occurred
                return;
            }

            BoneyState state = new BoneyState(serverState.get_max_slot());


            // server setup
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
                Services = { CompareAndSwapService.BindService(new CompareAndSwapImpl(state, paxosFrontend, serverState)), 
                             PaxosService.BindService(new PaxosImpl(state))},

                Ports = { new ServerPort(host, port, ServerCredentials.Insecure) }
            };

            server.Start();

            Console.WriteLine("ChatServer server listening on port " + port);

            // wait until starting time
            TimeSpan wait_time = serverState.get_starting_time() - DateTime.Now;
            Task slotStart = Task.Delay((int) wait_time.TotalMilliseconds);
            await slotStart;

            // TODO : need to call setup timeslot in the begining of each timeslot
            while (serverState.has_next_slot()) {
                
                // setup current time slot
                serverState.setup_timeslot();
                Console.WriteLine("Leader" + serverState.is_coordinator());

                // wait for the next time slot
                DateTime slot_beggining = serverState.get_starting_time() + (serverState.get_delta() * serverState.get_current_slot());
                wait_time = slot_beggining - DateTime.Now;

                slotStart = Task.Delay(wait_time);
                await slotStart;
            }

            Console.WriteLine("No more timeslots to execute");
            Console.WriteLine("Press any key to stop the server...");

            Console.ReadKey();
            server.ShutdownAsync().Wait();
        }
    }
}