using Grpc.Core;
using System.ComponentModel.Design;

namespace Bank
{
    public class BankCommand //TODO: Good idea???
    {
        public enum CommandType
        {
            Deposit,
            Withdrawal,
            ReadBalance
        };

        CommandType _type;
        private static int _clientId = 0;
        private static int _sequence_number = 1;
        private Tuple<int, int> _id;

        public BankCommand(int clientId, int sequence_number, CommandType type) {
            _id = new Tuple<int, int>(clientId, sequence_number);
            _type = type;
        }

        public Tuple<int, int>  getCommandId() {
            return _id;
        }
    }

    internal class BankServiceImpl : BankService.BankServiceBase
    {
        BankState _bankState;
        ServerState _serverState;
        BankFrontend _bankFrontend;

        public BankServiceImpl(BankState bankState, ServerState serverState, BankFrontend bankFrontend) {
            _bankState = bankState;
            _serverState = serverState;
            _bankFrontend = bankFrontend;
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
        }

        // deposit funcionality
        public override Task<DepositReply> Deposit
            (DepositRequest request, ServerCallContext context) {

            return Task.FromResult(do_deposit(request));
        }

        private DepositReply do_deposit(DepositRequest request) {
            // TODO : final vertion uses 2 phase commit to order commands

            var requestId = new CommandId
            {
                ClientId = request.Id.ClientId,
                ClientSequenceNumber = request.Id.ClientSequenceNumber,
            };

            // TODO : locks
            var type = BankCommand.CommandType.Deposit;
            var bankCommand = new BankCommand(requestId.ClientId, requestId.ClientSequenceNumber, type);
            _serverState.addUnordered(bankCommand);

            if (_serverState.is_coordinator()) {
                _bankFrontend.doCommand(requestId);
            }


            /*
            TODO 
            float amount = request.Amount;
            //_bankState.deposit(amount);
            return new DepositReply { Status = ResponseStatus.Ok };
            */

            return new DepositReply { };
        }


        // withdrawal funcionality
        public override Task<WithdrawalReply> Withdrawal
            (WithdrawalRequest request, ServerCallContext context) {

            return Task.FromResult(do_withdrawal(request));
        }

        private WithdrawalReply do_withdrawal(WithdrawalRequest request) {
            // TODO : final vertion uses 2 phase commit to order commands
            var requestId = new CommandId 
            {
                ClientId = request.Id.ClientId,
                ClientSequenceNumber = request.Id.ClientSequenceNumber,
            };

            // TODO : locks
            var type = BankCommand.CommandType.Withdrawal;
            var bankCommand = new BankCommand(requestId.ClientId, requestId.ClientSequenceNumber, type);
            _serverState.addUnordered(bankCommand);

            if (_serverState.is_coordinator()) {
                _bankFrontend.doCommand(requestId);
            }

            /*
            TODO :
            float amount = request.Amount;
            //bool succeeded = _bankState.withdrawal(amount);
            ResponseStatus status = ResponseStatus.Ok;
            //if (!succeeded) {
            //    status = ResponseStatus.NoFunds;
            //}

            return new WithdrawalReply { Status = status};
            */

            return new WithdrawalReply { };
        }


        // readBalance funcionality
        public override Task<ReadBalanceReply> ReadBalance
            (ReadBalanceRequest request, ServerCallContext context) {

            return Task.FromResult(do_readBalance(request));
        }

        private ReadBalanceReply do_readBalance(ReadBalanceRequest request) {
            // TODO : final vertion uses 2 phase commit to order commands
            var requestId = new CommandId
            {
                ClientId = request.Id.ClientId,
                ClientSequenceNumber = request.Id.ClientSequenceNumber,
            };

            // TODO : locks
            var type = BankCommand.CommandType.ReadBalance;
            var bankCommand = new BankCommand(requestId.ClientId, requestId.ClientSequenceNumber, type);
            _serverState.addUnordered(bankCommand);

            if (_serverState.is_coordinator()) {
                _bankFrontend.doCommand(requestId);
            }

            /*
            TODO : 
            float balance = _bankState.readBalance();
            
            return new ReadBalanceReply { Balance = balance };
            */

            return new ReadBalanceReply { };
        }

    }
}
