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
                
                //TODO : do argument verifications ??
                switch (command) {
                    case "P":
                        if (parts[2].Equals("client")) { continue; }

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
            ServerState serverState = new ServerState();
            //string config_path = @"..\..\..\..\..\configuration_sample.txt";
            string config_path = @"C:\Users\tomas\OneDrive\Ambiente de Trabalho\Uni\4Ano\P1\PADI\projeto\configuration_sample.txt";

            if (! processInput(args, config_path, serverState)) {
                // error processing input occurred
                return;
            }

            BankFrontend bankFrontend = new BankFrontend(serverState);

            // wait until starting time
            TimeSpan wait_time = serverState.get_starting_time() - DateTime.Now;
            Thread.Sleep((int) wait_time.TotalMilliseconds);


            // TODO : channel timeuot
            while (serverState.has_next_slot())
            {
                serverState.setup_timeslot();

                Console.WriteLine("Bank : sending comapare and swap for leader " + serverState.get_id());
                bankFrontend.compareAndSwap(serverState.get_current_slot(), serverState.get_id());
                
                // sleep until the next slot begins
                DateTime slot_beggining = serverState.get_starting_time() + (serverState.get_delta() * serverState.get_current_slot());
                wait_time = slot_beggining - DateTime.Now;

                Thread.Sleep(wait_time);

                /*
                // TODO : bank functionality
                if (lider)
                {
                    ordenar mensagens
                    enviar ordem para outros banks
                }
                */
            }
        }
    }
}