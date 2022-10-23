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
            return new DepositReply();
        }


        // withdrawal funcionality
        public override Task<WithdrawalReply> Withdrawal
            (WithdrawalRequest request, ServerCallContext context) {

            return Task.FromResult(do_withdrawal(request));
        }

        private WithdrawalReply do_withdrawal(WithdrawalRequest request) {
            return new WithdrawalReply();
        }


        // readBalance funcionality
        public override Task<ReadBalanceReply> ReadBalance
            (ReadBalanceRequest request, ServerCallContext context) {

            return Task.FromResult(do_readBalance(request));
        }

        private ReadBalanceReply do_readBalance(ReadBalanceRequest request) {

            return new ReadBalanceReply();
        }

    }
}
