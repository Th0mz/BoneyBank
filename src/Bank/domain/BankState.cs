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
        private object mutex = new object();

        public BankState() { }

        public bool deposit(float value) {
            lock (mutex) {
                balance += value;
                return true;
            }
        }

        public bool withdrawal(float value) {
            lock (mutex)
            {
                if (balance - value < 0) {
                    return false;
                }

                balance -= value;
                return true;
            }
        }

        public float readBalance() {
            lock (mutex) {
                return balance;
            }
        }
    }
}
