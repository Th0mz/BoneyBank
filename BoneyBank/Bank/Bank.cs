using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bank 
{   
    class Bank {

        static void Main(string[] args) {
            BankFrontend bankFrontend = new BankFrontend();
            // get arguments from args, check them and then initialize them

            bankFrontend.compareAndSwap(1, 2);
            
        }
    }
}