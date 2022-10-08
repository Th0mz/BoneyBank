﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank
{
    internal class BankState
    {
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
