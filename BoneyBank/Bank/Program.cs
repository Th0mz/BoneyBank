using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bank 
{   
    class Program {
        static void Main(string[] args) {
            BankFrontend bankFrontend = new BankFrontend();

            bankFrontend.compareAndSwap(1, 2);
            
        }
    }
}