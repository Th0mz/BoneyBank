using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client
{
    class Client
    {
        private static bool processInput(string[] args, string path, ClientState clientState)
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
            clientState.set_id(id);
            foreach (string line in File.ReadAllLines(path)) {
                string[] parts = line.Split(' ');

                string command = parts[0];

                switch (command)
                {
                    case "P":
                        if (parts[2].Equals("client") || parts[2].Equals("boney")) { continue; }

                        clientState.add_server(parts[1], parts[3]);
                        break;
                }
            }

            return true;
        }

        static void Main(string[] args)
        {
            ClientState state = new ClientState();
            ClientFrontend clientFrontend = new ClientFrontend(state);

            string config_path = @"..\..\..\..\..\configuration_sample.txt";
            //string config_path = @"C:\Users\tomas\OneDrive\Ambiente de Trabalho\Uni\4Ano\P1\PADI\projeto\configuration_sample.txt";
            string script_path = @"C:\Users\tomas\OneDrive\Ambiente de Trabalho\Uni\4Ano\P1\PADI\projeto\bank_client_script_sample.txt";
            
            if (!processInput(args, config_path, state)) {
                // error proceseing input
                return;
            }

            // user input process
            foreach (string command_line in File.ReadLines(script_path))
            {
                if (command_line == null) {
                    continue;
                }

                Console.WriteLine("===============\nExecuting : " + command_line);

                float amount;
                string[] parts = command_line.Split(' ');

                string command = parts[0];
                switch (command)
                {
                    case "D":
                        // argument check
                        if (parts.Length != 2) {
                            Console.WriteLine("Error : Invalid number of arguments");
                            continue;
                        }

                        if (!float.TryParse(parts[1], out amount)) {
                            Console.WriteLine("Error : Argument must be of type float");
                            continue; 
                        }

                        // remote call
                        clientFrontend.deposit(amount);
                        break;

                    case "W":
                        // argument check
                        if (parts.Length != 2) {
                            Console.WriteLine("Error : Invalid number of arguments");
                            continue; 
                        }

                        if (!float.TryParse(parts[1], out amount)) {
                            Console.WriteLine("Error : Argument must be of type float"); 
                            continue;
                        }

                        // remote call
                        clientFrontend.withdrawal(amount);
                        break;

                    case "R":
                        // argument check
                        if (parts.Length != 1) {
                            Console.WriteLine("Error : Invalid number of arguments");
                            continue;
                        }

                        // remote call
                        clientFrontend.readBalance();
                        break;

                    case "S":
                        // argument check
                        if (parts.Length != 2) { 
                            Console.WriteLine("Error : Invalid number of arguments");
                            continue; 
                        }

                        int milliseconds;
                        if (!int.TryParse(parts[1], out milliseconds)) {
                            Console.WriteLine("Error : Argument must be of type int"); 
                            continue;
                        }

                        // execute local functionality
                        Console.WriteLine("Sleep time started...");
                        Thread.Sleep(milliseconds);
                        Console.WriteLine("Sleep time ended...");
                        break;

                    case "Q":
                        return;

                    default:
                        Console.WriteLine("Error : command not found");
                        break;
                }
            }

            Console.WriteLine("\nClient script ended");
            Console.WriteLine("Press any key to close the client...");

            Console.ReadKey();
        }
    }
}