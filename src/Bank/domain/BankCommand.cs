using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Transactions;

namespace Bank
{

    public class BankCommand {
        
        private Tuple<int, int> _id;
        protected bool _is_commited = false;
        protected bool _is_applied = false;
        protected int _assignment_slot;
        protected BankState _state;

        public BankCommand(int clientId, int sequence_number, BankState state) {
            _id = new Tuple<int, int>(clientId, sequence_number);
            _state = state;
        }

        public Tuple<int, int> getCommandId() { return _id; }

        public int getClientId() { return _id.Item1; }
        public int getSequenceNumber() { return _id.Item2; }

        public bool is_commited() { return _is_commited; }

        public bool is_applied() { return _is_applied; }

        public void set_commited() { _is_commited = true; }
        public void set_applied() { _is_applied = true; }

        public int get_assignment_slot() { return _assignment_slot; }
        public void set_assignment_slot(int newAssignmentSlot) { _assignment_slot = newAssignmentSlot; }

        public virtual void execute () {
            throw new NotImplementedException();
        }
    }
     
    public class WithdrawalCommand : BankCommand
    {
        bool no_balance;
        float _amount;
        public WithdrawalCommand (int clientId, int sequence_number, float amount, BankState state) : 
            base (clientId, sequence_number, state) {
            
            _amount = amount;
        }

        public bool get_result() {
            return no_balance;
        }

        public override void execute ()
        {
            no_balance = _state.withdrawal(_amount);
            _is_applied = true;
        }
    }


    public class DepositCommand : BankCommand
    {
        float _amount;
        public DepositCommand(int clientId, int sequence_number, float amount, BankState state) :
            base(clientId, sequence_number, state) {
            
            _amount = amount;
        }

        public void get_result() {
            throw new NotImplementedException();
        }

        public override void execute()
        {
            _state.deposit(_amount);
            _is_applied = true;
        }
    }


    public class ReadBalanceCommand : BankCommand
    {
        private float balance = -1;
        public ReadBalanceCommand(int clientId, int sequence_number, BankState state) : 
            base(clientId, sequence_number, state) { }

        public float get_result () {
            return balance;
        }

        public override void execute()
        {
            balance = _state.readBalance();
            _is_applied = true;
        }
    }
}