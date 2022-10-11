using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace Bank 
{   
    class Bank {

        private static bool processInput (string[] args, ServerState serverState) {
            // check arugments 
            if (args.Length != 0) {
                Console.WriteLine("Error : invalid number of arguments");
                return false;
            }

            string path = @"C:\Users\tomas\OneDrive\Ambiente de Trabalho\Uni\4Ano\P1\PADI\projeto\configuration_sample.txt";
            if (!File.Exists(path)) {
                Console.WriteLine("Error : invalid path");
                return false;
            }

            int id = 4;
            /*if (!int.TryParse(args[1], out id)) {
                Console.WriteLine("Error : process id is not a number");
                return false;
            }*/

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
            if (! processInput(args, serverState)) {
                // error processing input occurred
                return;
            }

            Console.ReadKey();
            BankFrontend bankFrontend = new BankFrontend(serverState);
            bankFrontend.compareAndSwap(1, 2);
            

        }
    }
}