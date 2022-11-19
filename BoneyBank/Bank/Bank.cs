using Bank.implementations;
using Grpc.Core;
using Grpc.Core.Interceptors;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

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
                Console.WriteLine("Error : invalid configuration file path");
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
                Services = { BankService.BindService(new BankServiceImpl(bankState, serverState, bankFrontend)).Intercept(bankServiceServerInterceptor),
                             BankPaxos.BindService(new BankPaxosServiceImpl(serverState)).Intercept(bankPaxosServerInterceptor)},
                Ports = { new ServerPort(host, port, ServerCredentials.Insecure) }
            };

            server.Start();

            Console.WriteLine("Bank server listening on port " + port);

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
             
            DateTime starting_time = serverState.get_starting_time();
            while (serverState.has_next_slot())
            {
                int new_coord, current_slot;
                int coord, id;
                lock (serverState.currentSlotLock) {
                    lock (serverState.coordinatorLock) {
                        int tentative_coordinator = serverState.setup_timeslot();
                        current_slot = serverState.get_current_slot();

                        Console.WriteLine("> Sending comapare and swap for leader " + tentative_coordinator);
                        new_coord = bankFrontend.compareAndSwap(current_slot, tentative_coordinator);
                        
                        coord = serverState.get_coordinator_id();
                        id = serverState.get_id();

                        serverState.set_coordinator_id(new_coord);
                        serverState.add_coordinator_id(current_slot, new_coord);
                        Monitor.PulseAll(serverState.coordinatorLock);

                    }
                }

                // check if the coordinator changed and it is the new coordinator
                if (coord != -1 && new_coord != coord && new_coord == id) {
                    serverState.cleanupLock.EnterWriteLock();
                    try {
                        bankFrontend.doCleanup();
                    } finally {
                        serverState.cleanupLock.ExitWriteLock();
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


            Console.ReadKey();

            server.ShutdownAsync().Wait();
        }
    }
}