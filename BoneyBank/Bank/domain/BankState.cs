using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank
{
    public class BankState
    {
        // TODO : must syncronize with read/write locks
        private float balance = 0;

        public BankState() { }

        public bool deposit(float value) {
            balance += value;
            return true;
        }

        public bool withdrawal(float value) {
            if (balance - value < 0) {
                return false;
            }

            balance -= value;
            return true;
        }

        public float readBalance() {
            return balance;
        }
    }
}
