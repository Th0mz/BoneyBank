﻿using Grpc.Core;
using System.ComponentModel.Design;

namespace Bank
{

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
            var bankCommand = new DepositCommand(requestId.ClientId, requestId.ClientSequenceNumber, request.Amount, _bankState);
            _serverState.addUnordered(bankCommand);

            ServerStatus server = ServerStatus.Backup;
            if (_serverState.is_coordinator()) {
                server = ServerStatus.Primary;
                _bankFrontend.doCommand(requestId);
            }

            lock (bankCommand) {
                while (!bankCommand.is_commited()) {
                    Monitor.Wait(bankCommand);
                }

            }

            return new DepositReply { Status = ResponseStatus.Ok, Server = server };
        }


        // withdrawal funcionality
        public override Task<WithdrawalReply> Withdrawal
            (WithdrawalRequest request, ServerCallContext context) {

            return Task.FromResult(do_withdrawal(request));
        }

        private WithdrawalReply do_withdrawal(WithdrawalRequest request) {
            Console.WriteLine("Starting do_withdrawal");
            // TODO : final vertion uses 2 phase commit to order commands
            var requestId = new CommandId 
            {
                ClientId = request.Id.ClientId,
                ClientSequenceNumber = request.Id.ClientSequenceNumber,
            };

            // TODO : locks
            Console.WriteLine("Created command");
            var bankCommand = new WithdrawalCommand(requestId.ClientId, requestId.ClientSequenceNumber, request.Amount,_bankState);
            _serverState.addUnordered(bankCommand);

            ServerStatus server = ServerStatus.Backup;
            if (_serverState.is_coordinator()) {
                server = ServerStatus.Primary;
                _bankFrontend.doCommand(requestId);
            }
            Console.WriteLine("do_command completed");

            lock (bankCommand) {
                while (!bankCommand.is_commited()) {
                    Console.WriteLine("waiting");
                    Monitor.Wait(bankCommand);
                }
            }

            Console.WriteLine("outside");


            bool succeeded = bankCommand.get_result();
            ResponseStatus status = ResponseStatus.Ok;
            if (!succeeded) {
                status = ResponseStatus.NoFunds;
            }

            Console.WriteLine("return");

            return new WithdrawalReply { Status = status, Server = server };
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
            var bankCommand = new ReadBalanceCommand(requestId.ClientId, requestId.ClientSequenceNumber, _bankState);
            _serverState.addUnordered(bankCommand);

            ServerStatus server = ServerStatus.Backup;
            if (_serverState.is_coordinator()) {
                server = ServerStatus.Primary;
                _bankFrontend.doCommand(requestId);
            }

            lock (bankCommand) {
                while (!bankCommand.is_commited()) {
                    Monitor.Wait(bankCommand);
                }
            }

            float balance = bankCommand.get_result();
            return new ReadBalanceReply { Balance = balance, Server = server };
        }

    }
}
