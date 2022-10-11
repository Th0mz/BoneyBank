using System.Diagnostics;

public class Puppetmaster {

    private void handlePCommand(string commandLine) {
        /*ver entre boney a bank e criar processo com isso*/
        string[] cmds = commandLine.Split(' ');

        if( cmds[2].Equals("boney") ) {
            Console.WriteLine("boney");

            /*var processBoney = new Process {
                 StartInfo = new ProcessStartInfo
                     {
                         FileName = @"..\..\..\..\Boney\bin\Debug\net6.0\Boney.exe",
                         Arguments = "behavior query SymlinkEvaluation",
                         UseShellExecute = false, RedirectStandardOutput = true,
                         CreateNoWindow = true
                     }
             }*/

            var processBoney = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = @"..\Boney\bin\Debug\net6.0\Boney.exe",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            processBoney.Start();

        }
        else if( cmds[2].Equals("bank")) {
            Console.WriteLine("bank");

            /*var processBank = new Process {
                StartInfo = new ProcessStartInfo
                    {
                        FileName = @"..\..\..\..\Bank\bin\Debug\net6.0\Bank.exe",
                        Arguments = "behavior query SymlinkEvaluation",
                        UseShellExecute = false, RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
            }*/
      
            var processBank = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = @"..\..\..\..\Bank\bin\Debug\net6.0\Bank.exe",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            processBank.Start();
            /*fazer a cena de criar o processo com path tipo @"..\..\Boney\bin\Debug\netcoreapp3.1\Boney.exe";*/

        }
        else if( cmds[2].Equals("client")) {

        }
    }

    private void handleTCommand(string commandLine) {
        /*chamada rpc para isso*/
    }
    
    private void handleDCommand(string commandLine) {
        /*chamada rpc para isso*/
    }

}

class Program {
    public static void Main(string []args) {
        /*começar Bank e Boney instances e depois ler comandos*/
        /*sempre que ler um novo P cria se um processo novo???*/
        /*
        foreach (string line in File.ReadAllLines(path))
        {
            string[] parts = line.Split(' ');

            string command = parts[0];

            //TODO : do argument verifications ??
            switch (command)
            {
                case "P":
                    if (parts[2].Equals("client")) { continue; }

                    // TODO : check errors
                    //lançar servidor com url e id
                    serverState.add_server(parts[1], parts[2], parts[3]);
                    break;
                default:
                    break;
            }
        }

        */
        

    }
}
