using Grpc.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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


        //============================================================
        //======================== TENTATIVE =========================
        //============================================================

        //receive tentative ordered commands
        public override Task<TentativeReply> Tentative(TentativeRequest request, ServerCallContext context) {
            return Task.FromResult(do_tentative(request));
        }

        private TentativeReply do_tentative(TentativeRequest request) {
            Console.WriteLine("{id="+request.RequestId.ClientId+";seq_number="+request.SequenceNumber+"} " + "Processing tentative");
            Tuple<int, int> commandId = new(request.RequestId.ClientId, request.RequestId.ClientSequenceNumber);
            int assignment_slot = request.AssignmentSlot;
            int sender_id = request.SenderId;
            int sequence_number = request.SequenceNumber;

            //server might still not have received the command from the client
            lock(_serverState.allCommandsLock) {
                while (!_serverState.command_exists(commandId)) {
                    Monitor.Wait(_serverState.allCommandsLock);
                }
            }

            //only responds with ack after all the previous commands were received
            lock (_serverState.lastSequentialLock) {
                Console.WriteLine("{id=" + request.RequestId.ClientId + ";seq_number=" + request.SequenceNumber + "} " + "only responds with ack after all the previous commands were received");
                while (_serverState.getLastSequential() < sequence_number - 1) {
                    Console.WriteLine("{id=" + request.RequestId.ClientId + ";seq_number=" + request.SequenceNumber + "} " + "waiting for previous commands");
                    Monitor.Wait(_serverState.lastSequentialLock);
                }
            }

            //TODO: completely correct???
            lock (_serverState.currentSlotLock) {

                //check if sender was the primary for the slot of the assignment
                Console.WriteLine("{id=" + request.RequestId.ClientId + ";seq_number=" + request.SequenceNumber + "} " + "Check slot leader");
                int coordinator_assignment_slot = _serverState.get_coordinator_id(assignment_slot);
                if (coordinator_assignment_slot != sender_id)
                {
                    return new TentativeReply { Ack = false };
                }

                //check if the coordinator has changed since
                Console.WriteLine("{id=" + request.RequestId.ClientId + ";seq_number=" + request.SequenceNumber + "} " + "Check if the coordinator has changed since");
                for (int slot = assignment_slot; slot < _serverState.get_current_slot(); slot++) {
                    if (_serverState.get_coordinator_id(slot) != sender_id) {
                        return new TentativeReply { Ack = false };
                    }
                }                            

                lock (_serverState.unorderedLock) {
                    lock (_serverState.orderedLock) {
                        lock (_serverState.lastAppliedLock) {
                            lock (_serverState.lastTentativeLock) {
                                lock (_serverState.lastSequentialLock) {
                                    Console.WriteLine("{id=" + request.RequestId.ClientId + ";seq_number=" + request.SequenceNumber + "} " + "Inside locks");
                                    //already applied
                                    if (sequence_number <= _serverState.get_last_applied()) {
                                        Console.WriteLine("{id=" + request.RequestId.ClientId + ";seq_number=" + request.SequenceNumber + "} " + "already applied");
                                        return new TentativeReply { Ack = false };
                                    }


                                    //update the last sequential
                                    //have to check from last applied because previous accepted might have
                                    //traded positions
                                    Console.WriteLine("{id=" + request.RequestId.ClientId + ";seq_number=" + request.SequenceNumber + "} " + "update the last sequential");
                                    for (int index = _serverState.get_last_applied() + 1;
                                        index <= _serverState.get_last_tentative(); index++) {
                                        
                                        if (_serverState.get_ordered_command(index) != null) {
                                            _serverState.setLastSequential(index);
                                            Monitor.PulseAll(_serverState.lastSequentialLock);
                                        } else { break; }
                                    }
                                    
                                    Console.WriteLine("{id=" + request.RequestId.ClientId + ";seq_number=" + request.SequenceNumber + "} " + "yoo");
                                    var previous_command_id = _serverState.get_ordered_command(sequence_number);
                                    if (previous_command_id != null) {
                                        Console.WriteLine("{id=" + request.RequestId.ClientId + ";seq_number=" + request.SequenceNumber + "} " + "previous command id != null");
                                        if (_serverState.get_command(previous_command_id).is_commited() ||
                                                _serverState.get_command(previous_command_id).get_assignment_slot() >= assignment_slot)
                                            return new TentativeReply { Ack = false };

                                        //Received a more recently sent command for an occupied log position that
                                        //isnt committed yet
                                        _serverState.addUnorderedId(previous_command_id);
                                        _serverState.addAcceptedCommand(commandId, sequence_number);
                                        _serverState.get_command(commandId).set_assignment_slot(assignment_slot);

                                    } else { //log is still empty
                                        Console.WriteLine("{id=" + request.RequestId.ClientId + ";seq_number=" + request.SequenceNumber + "} " + "previous command id == null");
                                        _serverState.addAcceptedCommand(commandId, sequence_number);
                                        _serverState.get_command(commandId).set_assignment_slot(assignment_slot);
                                    }

                                    //Can be sure the commands are sequential because only acks command when already
                                    //has all the previous commands
                                    if (sequence_number > _serverState.getLastSequential()) {
                                        _serverState.setLastSequential(sequence_number);
                                        Monitor.PulseAll(_serverState.lastSequentialLock);
                                    }

                                    Console.WriteLine("{id=" + request.RequestId.ClientId + ";seq_number=" + request.SequenceNumber + "} " + "Finished tentative with ack = true");

                                    return new TentativeReply { Ack = true };
                                }
                            }
                        }
                    }
                }
            }           
        }




        //============================================================
        //========================== COMMIT ==========================
        //============================================================

        //receive commit requests
        public override Task<CommitReply> Commit(CommitRequest request, ServerCallContext context) {
            return Task.FromResult(do_commit(request));
        }

        private CommitReply do_commit(CommitRequest request) {

            Console.WriteLine("{id=" + request.RequestId.ClientId + ";seq_number=" + request.SequenceNumber + "} " + "Processing commit");
            Tuple<int, int> commandId = new(request.RequestId.ClientId, request.RequestId.ClientSequenceNumber);
            int seqNumberToCommit = request.SequenceNumber;

            lock (_serverState.allCommandsLock) {
                while (!_serverState.command_exists(commandId)) {
                    Monitor.Wait(_serverState.allCommandsLock);
                }
            }

            lock (_serverState.unorderedLock) {
                lock (_serverState.orderedLock) { 
                    lock (_serverState.lastAppliedLock) {
                        lock (_serverState.lastTentativeLock) {
                            lock (_serverState.lastSequentialLock) {
                                int lastApplied = _serverState.get_last_applied();
                                int lastTentative = _serverState.get_last_tentative();
                                BankCommand commandToCommit = _serverState.get_command(commandId);
                                var commandIdInIndex = _serverState.get_ordered_command(seqNumberToCommit);

                                //Already has been commited; nothing to do
                                //Possible if the new coordinator sends new commmit for command already applied
                                Console.WriteLine("{id=" + request.RequestId.ClientId + ";seq_number=" + request.SequenceNumber + "} " + "Already has been applied");
                                if (seqNumberToCommit < lastApplied)
                                    return new CommitReply { };

                                //case when a commit comes from a previous view. Has to be added and only then applied
                                if (commandIdInIndex == null) {
                                    _serverState.addAcceptedCommand(commandId, seqNumberToCommit);

                                    if (seqNumberToCommit > lastTentative)
                                        _serverState.set_last_tentative(seqNumberToCommit);

                                    //TODO: put in an auxiliary function
                                    for (int index = _serverState.get_last_applied() + 1;
                                        index <= _serverState.get_last_tentative(); index++) {
                                        if (_serverState.get_ordered_command(index) != null) {
                                            _serverState.setLastSequential(index);
                                            Monitor.PulseAll(_serverState.lastSequentialLock);
                                        }
                                        else { break; }
                                    }
                                    

                                } else { //there is a command in given log position
                                    if (!commandIdInIndex.Equals(commandId)) { //command in log isnt the supposed one
                                        if (_serverState.get_command(commandIdInIndex).is_commited())
                                            return new CommitReply { };
                                        _serverState.addUnorderedId(commandIdInIndex);
                                        _serverState.addAcceptedCommand(commandId, seqNumberToCommit);
                                    }
                                }

                                commandToCommit = _serverState.get_command(seqNumberToCommit);
                                
                                commandToCommit.set_commited();

                                //apply all the commands possible
                                Console.WriteLine("{id=" + request.RequestId.ClientId + ";seq_number=" + request.SequenceNumber + "} " + "Loop throught the uncommited commands and execute them");
                                for (int index = lastApplied + 1; index <= lastTentative; index++) {
                                    Console.WriteLine("{id=" + request.RequestId.ClientId + ";seq_number=" + request.SequenceNumber + "} " + "Started command commit");
                                    BankCommand command = _serverState.get_command(index);
                                    if (command == null) break; //list of commands isnt sequential from this point on

                                    lock (command) {
                                        if (!command.is_commited()) break; //not all previous commands are committed. cant apply yet

                                        command.execute();
                                        _serverState.setLastApplied(index);
                                        Monitor.PulseAll(command);
                                    }

                                    Console.WriteLine("{id=" + request.RequestId.ClientId + ";seq_number=" + request.SequenceNumber + "} " + "Ended command commit");
                                }

                                return new CommitReply { };
                            }
                        }
                    }
                }               
            }
            
        }



        //============================================================
        //========================= CLEANUP ==========================
        //============================================================

        public override Task<CleanupReply> Cleanup(CleanupRequest request, ServerCallContext context) {
            return Task.FromResult(do_cleanup(request));
        }

        private CleanupReply do_cleanup(CleanupRequest request) {
            lock (_serverState.orderedLock) {
                lock (_serverState.lastTentativeLock) { 
                    CleanupReply reply = new CleanupReply { 
                                            HighestKnownSeqNumber = _serverState.get_last_tentative() };

                    for (int index = request.LastApplied; index < _serverState.get_last_tentative(); index++) { 
                        var command_id = _serverState.get_ordered_command(index);
                        if (command_id != null)
                        {
                            BankCommand command = _serverState.get_command(command_id);
                            reply.Accepted.Add(new CommandIdWithSeqNum
                            {
                                RequestId = new CommandId
                                {
                                    ClientId = command.getClientId(),
                                    ClientSequenceNumber = command.getSequenceNumber()
                                },
                                SequenceNumber = index
                            });
                        }   
                    }
                    return reply; 
                }
            }
        }

    }
}
