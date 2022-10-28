using Grpc.Core;
using System.ComponentModel.Design;

namespace Bank
{
    public class BankCommand //TODO: Good idea???
    {
        //TODO: Missing all the necessary values for each command
        public enum CommandType
        {
            Deposit,
            Withdrawal,
            ReadBalance
        }

        CommandType type;
        string commandId; //always identify commands by this unique id
        public BankCommand(CommandType t) {
            type = t;
        }
        public string getCommandId() {
            return commandId;
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
            _serverState.addUnordered(new BankCommand(BankCommand.CommandType.Deposit));
            _bankFrontend.doCommand(request.CommandId);
            float amount = request.Amount;
            
            _bankState.deposit(amount);
            return new DepositReply { Status = ResponseStatus.Ok };
        }


        // withdrawal funcionality
        public override Task<WithdrawalReply> Withdrawal
            (WithdrawalRequest request, ServerCallContext context) {

            return Task.FromResult(do_withdrawal(request));
        }

        private WithdrawalReply do_withdrawal(WithdrawalRequest request) {
            // TODO : final vertion uses 2 phase commit to order commands
            _serverState.addUnordered(new BankCommand(BankCommand.CommandType.Withdrawal));
            _bankFrontend.doCommand(request.CommandId);
            float amount = request.Amount;

            bool succeeded = _bankState.withdrawal(amount);
            ResponseStatus status = ResponseStatus.Ok;
            if (!succeeded) {
                status = ResponseStatus.NoFunds;
            }


            return new WithdrawalReply { Status = status};
        }


        // readBalance funcionality
        public override Task<ReadBalanceReply> ReadBalance
            (ReadBalanceRequest request, ServerCallContext context) {

            return Task.FromResult(do_readBalance(request));
        }

        private ReadBalanceReply do_readBalance(ReadBalanceRequest request) {
            // TODO : final vertion uses 2 phase commit to order commands
            _serverState.addUnordered(new BankCommand(BankCommand.CommandType.ReadBalance));
            _bankFrontend.doCommand(request.CommandId);
            float balance = _bankState.readBalance();
            
            return new ReadBalanceReply { Balance = balance };
        }

    }
}
