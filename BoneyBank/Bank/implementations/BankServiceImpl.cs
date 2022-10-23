using Grpc.Core;

namespace Bank
{
    internal class BankServiceImpl : BankService.BankServiceBase
    {
        BankState _bankState;
        public BankServiceImpl(BankState bankState) {
            _bankState = bankState;
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
        }

        // deposit funcionality
        public override Task<DepositReply> Deposit
            (DepositRequest request, ServerCallContext context) {

            return Task.FromResult(do_deposit(request));
        }

        private DepositReply do_deposit(DepositRequest request) {
            // TODO : final vertion uses 2 phase commit to order commands
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
            float balance = _bankState.readBalance();
            
            return new ReadBalanceReply { Balance = balance };
        }

    }
}
