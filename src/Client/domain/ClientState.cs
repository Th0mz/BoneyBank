using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class ClientState
    {
        int _id;
        int _sequenceNumber = 0;
        DateTime _starting_time;

        private Dictionary<int, BankServerConnection> _banks = new Dictionary<int, BankServerConnection>();

        public Dictionary<int, BankServerConnection> get_bank_servers () {
            return _banks;
        }

        public int get_id () {
            return _id;
        }

        public DateTime get_starting_time() {
            return _starting_time;
        }

        public int get_sequence_number () {
            return _sequenceNumber++;
        }

        public void set_id (int id) {
            _id = id;
        }

        public void set_starting_date(string starting_date) {
            _starting_time = Convert.ToDateTime(starting_date);
        }

        public bool add_server(string sid, string url) {
            int id;
            if (!int.TryParse(sid, out id)) {
                return false;
            }

            BankServerConnection connection = new BankServerConnection(url);
            _banks.Add(id, connection);

            return true;
        }
    }
}
