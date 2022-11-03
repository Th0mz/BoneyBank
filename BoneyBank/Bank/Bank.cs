using Bank.implementations;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Bank 
{   
    class Bank {

        private static bool processInput (string[] args, string path, ServerState serverState) {
            // check arugments 
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
            foreach (string line in File.ReadAllLines(path)) {
                string[] parts = line.Split(' ');

                string command = parts[0];
                
                switch (command) {
                    case "P":
                        if (parts[2].Equals("client")) { continue; }

                        serverState.add_server(parts[1], parts[2], parts[3]);
                        break;
                    case "T":
                        serverState.set_starting_date(parts[1]);
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

            BankState bankState = new BankState();
            ServerState serverState = new ServerState();
            string config_path = @"..\..\..\..\..\configuration_sample.txt";
            //string config_path = @"C:\Users\tomas\OneDrive\Ambiente de Trabalho\Uni\4Ano\P1\PADI\projeto\configuration_sample.txt";

            if (! processInput(args, config_path, serverState)) {
                // error processing input occurred
                return;
            }

            BankFrontend bankFrontend = new BankFrontend(serverState);
             
            // bank server setup
            string url = serverState.get_url();
            string[] urlSplit = url.Split(':');

            string host = urlSplit[1][2..];
            string _port = urlSplit[2];

            int port;
            if (!int.TryParse(_port, out port)) {
                Console.WriteLine("Error : Invalid port");
                return;
            }

            // create server side interceptors
            FrozenInterceptor bankServiceServerInterceptor = new FrozenInterceptor(serverState);
            serverState.add_server_interceptor(bankServiceServerInterceptor);
            FrozenInterceptor bankPaxosServerInterceptor = new FrozenInterceptor(serverState);
            serverState.add_server_interceptor(bankPaxosServerInterceptor);

            Server server = new Server {
                Services = { BankService.BindService(new BankServiceImpl(bankState, serverState, bankFrontend)),
                             BankPaxos.BindService(new BankPaxosServiceImpl(serverState))},
                Ports = { new ServerPort(host, port, ServerCredentials.Insecure) }
            };

            server.Start();

            Console.WriteLine("Boney server listening on port " + port);

            // wait until starting time
            TimeSpan wait_time = serverState.get_starting_time() - DateTime.Now;
            Task slotStart;
            if (wait_time > TimeSpan.Zero) {
                slotStart = Task.Delay((int)wait_time.TotalMilliseconds);
                await slotStart;
            }
            else {
                Console.WriteLine("Current process started ahead of starting time.");
                return;
            }
             
            Console.WriteLine("WHAT IS CURRENT SLOT?= " + serverState.get_current_slot());
            Console.WriteLine("WHAT IS CURRENT COORD?= " + serverState.get_coordinator_id());

            while (serverState.has_next_slot())
            {
                DateTime starting_time;
                int current_slot;
                lock (serverState.currentSlotLock) {
                    lock (serverState.coordinatorLock) {
                        serverState.setup_timeslot();
                        Console.WriteLine("[" + serverState.get_current_slot() + "] Bank : DID SETUP TIMESLOT ");
                        starting_time = serverState.get_starting_time();
                        current_slot = serverState.get_current_slot();

                        Console.WriteLine("[" + serverState.get_current_slot() + "] Bank : sending comapare and swap for leader " + serverState.get_coordinator_id());
                        int new_coord = bankFrontend.compareAndSwap(current_slot, serverState.get_coordinator_id());
                        serverState.set_coordinator_id(new_coord);
                    }
                }
                  
                // sleep until the next slot begins
                DateTime slot_beggining = starting_time + (serverState.get_delta() * current_slot);
                wait_time = slot_beggining - DateTime.Now;

                
                if (wait_time > TimeSpan.Zero) {
                    slotStart = Task.Delay(wait_time);
                    await slotStart;
                } 
            }

            Console.WriteLine("No more timeslots to execute");
            Console.WriteLine("Press any key to stop the server...");

            Console.ReadKey();

            server.ShutdownAsync().Wait();
        }
    }
}