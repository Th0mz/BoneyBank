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

        public BankPaxosServiceImpl(ServerState serverState) {
            _serverState = serverState;
        }

        //receive tentative ordered commands
        public override Task<TentativeReply> Tentative(TentativeRequest request, ServerCallContext context) {
            return Task.FromResult(doTentative(request));
        }

        private TentativeReply doTentative(TentativeRequest request) {
            Tuple<int, int> commandId = new(request.RequestId.ClientId, request.RequestId.ClientSequenceNumber);
            int assignment_slot = request.AssignmentSlot;
            int sender_id = request.SenderId;
            int sequence_number = request.SequenceNumber;
            
            //check if sender was the primary for the slot of the assignment
            int coordinator_assignment_slot = _serverState.get_coordinator_id(assignment_slot);
            if (!(coordinator_assignment_slot == sender_id))
                return new TentativeReply { Ack = false };

            //check if the coordinator has changed since
            for(int slot = assignment_slot; slot < _serverState.get_current_slot(); slot++) {
                if(_serverState.get_coordinator_id(slot) != sender_id)
                    return new TentativeReply { Ack = false };
            }

            //needed??? check if the sequence number is valid??
            //should be as the coordinator is the only one who assigns sequence numbers
            //if(_serverState.is_index_taken(sequence_number) || !_serverState.command_exists(commandId))
            //    return new TentativeReply { Ack = false };
            //sequence numbers are not definitive until commit

            _serverState.removeUnordered(commandId);
            _serverState.addOrdered(commandId);

            return new TentativeReply { Ack = true };
        }

        //receive commit requests
        public override Task<CommitReply> Commit(CommitRequest request, ServerCallContext context) {
            return Task.FromResult(doCommit(request));
        }

        private CommitReply doCommit(CommitRequest request) {
            //TODO:
            //tem de aplicar os comandos por ordem
            //o commit eh so do primeiro comando nao commited ou de um grupo random??

            /*
            lock (order_list)
            {
            */

            int lastCommited = _serverState.get_last_commited();
            int lastTentative = _serverState.get_last_tentative();

            for (int sequence_number = lastCommited; sequence_number < lastTentative; sequence_number++) {
                BankCommand command = _serverState.get_command(sequence_number);
                    
                lock (command) {
                    command.execute();
                    Monitor.PulseAll(command);
                }
            }
            
            return new CommitReply { };
        }

    }
}
