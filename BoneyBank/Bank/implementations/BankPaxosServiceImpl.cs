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

            lock (_serverState.lastTentativeLock) {
                //only responds with ack after all the previous commands were received
                if(sequence_number <= _serverState.get_last_commited())
                    return new TentativeReply { Ack = false };

                while (_serverState.get_last_tentative() < sequence_number - 1)
                {
                    Monitor.Wait(_serverState.lastTentativeLock);
                }
                _serverState.removeUnordered(commandId);
                _serverState.addOrdered(commandId);
            }            

            return new TentativeReply { Ack = true };
        }

        //receive commit requests
        public override Task<CommitReply> Commit(CommitRequest request, ServerCallContext context) {
            return Task.FromResult(doCommit(request));
        }

        private CommitReply doCommit(CommitRequest request) {
            //TODO:
            //DOES IT MATTER IN WHICH SLOT A SERVER IS COMMITING ITS REQUESTS
            //THAT GOT ACCEPTED???

            lock (_serverState.orderedLock)
            {
                int lastCommited = _serverState.get_last_commited();
                int lastTentative = _serverState.get_last_tentative();
                int seqNumberToCommit = request.SequenceNumber;

                Tuple<int, int> commandId = new(request.RequestId.ClientId, request.RequestId.ClientSequenceNumber);
                BankCommand commandToCommit = _serverState.get_command(seqNumberToCommit);

                //TODO:
                //DO NEED TO MAKE SURE THE COORDINATOR HAS NOT CHANGED SINCE THE SLOT
                //IN WHICH THE COMMIT WAS SENT. IS IT SUFFICENT TO GUARANTEE SAFETY???

                //Make sure that the command still holds given sequence number.
                if (commandToCommit.getCommandId() != commandId)
                    return new CommitReply { };

                //Already has been commited; nothing to do
                //Possible if the commits from a coordinator come out of order
                if (seqNumberToCommit < lastCommited)
                    return new CommitReply { };

                //When receiving a commit for command x we can be sure there will be...
                //a commit for all commands y for y < x, since backups only accept command x if
                //all commands y have already been acked
                //commit for x => majority ack for x => same majority ack for y => should commit y
                for (int index = lastCommited + 1; index <= seqNumberToCommit; index++) {
                    BankCommand command = _serverState.get_command(index);
                    
                    lock (command) {
                        command.execute();
                        Monitor.PulseAll(command);
                    }
                }
                _serverState.setLastCommited(seqNumberToCommit);
            
                return new CommitReply { };
            }
            
        }

    }
}
