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
            /*fazer a cena de criar o processo com path tipo @"..\..\..\..\Boney\bin\Debug\netcoreapp3.1\Boney.exe";*/

        }else if( cmds[2].Equals("bank")) {
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
        }
    }

    private void handleTCommand(string commandLine) {
        /*chamada rpc para isso*/
    }
    
    private void handleDCommand(string commandLine) {
        /*chamada rpc para isso*/
    }


    public void readCommand(string commandLine) {
        char cmd = commandLine[0];

        switch(cmd) {
            case 'P':
                handlePCommand(commandLine);
                Console.WriteLine("P");
                break;
            case 'T':
                handleTCommand(commandLine);
                Console.WriteLine("T");
                break;
            case 'D':
                handleDCommand(commandLine);
                Console.WriteLine("D");
                break;
            default:
                Console.WriteLine("ERROR: Command not found.");
                break;
        }
    }


}

class Program {
    public static void Main(string []args) {
        /*começar Bank e Boney instances e depois ler comandos*/
        /*sempre que ler um novo P cria se um processo novo???*/ 

        Puppetmaster master = new Puppetmaster(); 

        int i = 0;
        while(args != null) {
            master.readCommand(args[i]);
            i++;
        }      

    }
}
