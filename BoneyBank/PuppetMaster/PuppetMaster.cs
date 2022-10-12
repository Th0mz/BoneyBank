using System.Diagnostics;
using System.Runtime.InteropServices;

public class Puppetmaster {

    private static void startBoney(string id) {
        var processBoney = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = @"..\..\..\..\Boney\bin\Debug\net6.0\Boney.exe",
                UseShellExecute = true,
                CreateNoWindow = true,
                Arguments = id
            }
        };

        processBoney.Start();
    }

    private static void startBank(string id)
    {
        var processBank = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = @"..\..\..\..\Bank\bin\Debug\net6.0\Bank.exe",
                UseShellExecute = true,
                CreateNoWindow = true,
                Arguments = id
            }
        };
        processBank.Start();
    }


    public static void Main(string []args) {
        /*começar Bank e Boney instances e depois ler comandos*/
        /*sempre que ler um novo P cria se um processo novo???*/

        string config_path = @"..\..\..\..\..\configuration_sample.txt";

        foreach (string line in File.ReadAllLines(config_path)) {
            string[] parts = line.Split(' ');
            string command = parts[0];

            //TODO : do argument verifications ??
            switch (command) {
                case "P":
                    if (parts[2].Equals("client")) { continue; }

                    else if (parts[2].Equals("boney")) {
                        startBoney(parts[1]);
                    }

                    else if (parts[2].Equals("bank")) {
                        startBank(parts[1]);
                    }
                    break;

                default:
                    break;
            }
        }
    }
}
