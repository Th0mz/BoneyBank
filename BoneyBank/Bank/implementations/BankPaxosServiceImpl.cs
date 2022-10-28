using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.implementations
{
    internal class BankPaxosServiceImpl : BankPaxos.BankPaxosBase
    {
        ServerState _serverState;

        public BankPaxosServiceImpl(ServerState serverState)
        {
            _serverState = serverState;
        }

        //receive tentative ordered commands
        public override Task<TentativeReply> Tentative(TentativeRequest request, ServerCallContext context)
        {
            return Task.FromResult(doTentative(request));
        }

        private TentativeReply doTentative(TentativeRequest request)
        {
            //TODO:
            //ver quem mandou
            //ver se o command existe na lista unordered no server state
            //ver se o seq number esta vazio na lista de ordered no server state
            //se for valido responder com "ack=true", remover de unordered e adicionar a ordered

            return new TentativeReply { };
        }

        //receive commit requests
        public override Task<CommitReply> Commit(CommitRequest request, ServerCallContext context)
        {
            return Task.FromResult(doCommit(request));
        }

        private CommitReply doCommit(CommitRequest request)
        {
            //TODO:
            //tem de aplicar os comandos por ordem
            //o commit eh so do primeiro comando nao commited ou de um grupo random??

            return new CommitReply { };
        }

    }
}
