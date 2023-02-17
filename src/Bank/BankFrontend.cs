using Grpc.Core.Interceptors;
using Grpc.Core;
using Grpc.Net.Client;
using System.ComponentModel.Design;

namespace Bank
{
    public class BankFrontend {
        private ServerState _serverState;
        private bool initialized_connections = false;

        public BankFrontend (ServerState serverState) {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            _serverState = serverState; 
        }

        public void setup_connections() {
            if (!initialized_connections) {
                foreach (BankPaxosServerConnection serverConnection in _serverState.get_bank_servers().Values) {
                    serverConnection.setup_stub(_serverState);
                }

                initialized_connections = true;
            }
        }

        // this method shouldn't be async
        public int compareAndSwap (int slot, int leader) {
            CompareAndSwapRequest request = new CompareAndSwapRequest { Slot = slot, Leader = leader };

            List<Task<CompareAndSwapReply>> replies = new List<Task<CompareAndSwapReply>>(); 
            foreach (var client in _serverState.get_boney_servers().Values){
                var reply_async = client.CompareAndSwapAsync(request).ResponseAsync;
                replies.Add(reply_async);
            }


            var task_reply = Task.WhenAny(replies).Result;
            var reply = task_reply.Result;

            // remove recieved reply
            replies.Remove(task_reply);

            return reply.Leader;
        }

       public void doCommand(CommandId commandId)
        {
            //create the grpc channels to other bank servers if they dont already exist
            setup_connections();

            int id = _serverState.get_id();
            int sequence_number;
            int assignment_slot;
            List<Task<TentativeReply>> tentative_replies;
            int count = 0;
            int number_servers = _serverState.get_bank_servers().Count();

            lock (_serverState.currentSlotLock) { 
                lock (_serverState.lastTentativeLock) {
                    lock (_serverState.nextSequenceNumberLock) { 
                        sequence_number = _serverState.getAndIncNextSequenceNumber();
                        assignment_slot = _serverState.get_current_slot();
                    }
                }
            }

            tentative_replies = tentative(id, sequence_number, assignment_slot, commandId);

            while (tentative_replies.Any() && (count < (number_servers / 2) + 1))
            {
                var task_reply = Task.WhenAny(tentative_replies).Result;
                var reply = task_reply.Result;

                // check if the command was acked by the other banks
                if (reply.Ack) { count++; }

                // remove received reply
                tentative_replies.Remove(task_reply);
            }

            if(count >= (number_servers / 2) + 1) {
                commit(commandId, sequence_number);
            } 
        }

        public void doCleanup () {

            setup_connections();

            int number_servers = _serverState.get_bank_servers().Count();
            int count = 0;
            List<Task<CleanupReply>> cleanup_replies;

            int last_applied;
            int slot;
            lock (_serverState.currentSlotLock) { 
                lock (_serverState.lastAppliedLock) {
                    last_applied = _serverState.get_last_applied();
                    slot = _serverState.get_current_slot();
                }
            }

            cleanup_replies = cleanup(last_applied, slot);
            
            //TODO: create 'previous' list <commandId, sequenceNumber>
            //HashSet<Tuple<int, int>> accepted = new();
            Dictionary<int, CommandIdWithSeqNum> accepted = new();
            int highestSeqNumReplies = -1;

            while (cleanup_replies.Any() && (count < (number_servers / 2) + 1))
            {
                var task_reply = Task.WhenAny(cleanup_replies).Result;
                var reply = task_reply.Result;
                count++;

                lock (_serverState.lastTentativeLock) {
                    if (reply.HighestKnownSeqNumber > _serverState.get_last_tentative())
                        _serverState.set_last_tentative(reply.HighestKnownSeqNumber);
                }

                lock (_serverState.nextSequenceNumberLock) {
                    if (reply.HighestKnownSeqNumber > highestSeqNumReplies)
                        highestSeqNumReplies = reply.HighestKnownSeqNumber;
                    if (reply.HighestKnownSeqNumber > _serverState.getNextSequenceNumber())
                        _serverState.setNextSequenceNumber(reply.HighestKnownSeqNumber + 1);
                }

                //TODO: make sure it's not necessary given we use perfect channels
                /*List<Tuple<int, int>> committed = new();
                foreach(var commandId in reply.Committed) {
                    committed.Add(new Tuple<int, int>(commandId.ClientId, commandId.ClientSequenceNumber));
                }*/

                foreach (var commandId in reply.Accepted) {
                    //accepted.Add(new Tuple<int, int>(commandId.ClientId, commandId.ClientSequenceNumber));
                    if (accepted.ContainsKey(commandId.SequenceNumber)) {
                        if (commandId.AssignmentSlot > accepted[commandId.SequenceNumber].AssignmentSlot)
                            accepted[commandId.SequenceNumber] = commandId;
                    } else {
                        accepted[commandId.SequenceNumber] = commandId;
                    }
                    
                    _serverState.removeUnordered(new Tuple<int, int>(commandId.RequestId.ClientId, commandId.RequestId.ClientSequenceNumber));
                }
                
                // remove received reply
                cleanup_replies.Remove(task_reply);
            }

            for(int index = last_applied + 1; index < highestSeqNumReplies; index++) {
                if (accepted.ContainsKey(index)) {
                        
                    doPreviousCommand(accepted[index].RequestId, accepted[index].SequenceNumber);
                }
            }

            
            for (int index = 0; index < highestSeqNumReplies; index++) {
                var ordered_commands = _serverState.getOrderedCommands();
                if (ordered_commands.ContainsKey(index)) {
                    if (!accepted.ContainsKey(index)) {
                        var id = ordered_commands[index];
                        doCommand(new CommandId { ClientId = id.Item1, ClientSequenceNumber = id.Item2});
                    }
                }
            }


            foreach (var commandId in _serverState.getUnorderedCommands()) {
                doCommand(new CommandId { ClientId = commandId.Item1, ClientSequenceNumber = commandId.Item2 });
            }
            //TODO: can only set the server as coordinator when it has finished cleanup

        }


        public void doPreviousCommand(CommandId commandId, int sequence_number) {
            //create the grpc channels to other bank servers if they dont already exist
            setup_connections();

            //TODO: locks
            int id = _serverState.get_id();
            int number_servers = _serverState.get_bank_servers().Count();
            int assignment_slot;
            int count = 0;
            List<Task<TentativeReply>> tentative_replies;

            lock (_serverState.currentSlotLock) {
                assignment_slot = _serverState.get_current_slot();
            }
            
            tentative_replies = tentative(id, sequence_number, assignment_slot, commandId);
            
            while (tentative_replies.Any() && (count < (number_servers / 2) + 1))
            {
                var task_reply = Task.WhenAny(tentative_replies).Result;
                var reply = task_reply.Result;

                // check if the command was acked by the other banks
                if (reply.Ack) { count++; }

                // remove received reply
                tentative_replies.Remove(task_reply);
            }

            if (count >= (number_servers / 2) + 1) {
                commit(commandId, sequence_number);
            }
        }


        public List<Task<TentativeReply>> tentative(int id, int sequence_number, int assignment_slot, CommandId commandId)
        {
            setup_connections();
            TentativeRequest request = new TentativeRequest
            {
                RequestId = commandId,
                SequenceNumber = sequence_number,
                AssignmentSlot = assignment_slot,
                SenderId = id

            };

            List<Task<TentativeReply>> replies = new List<Task<TentativeReply>>();
            foreach (BankPaxosServerConnection connection in _serverState.get_bank_servers().Values)
            {
                var client = connection.get_client();
                var reply = client.TentativeAsync(request).ResponseAsync;
               
                replies.Add(reply);
            }

            return replies;
        }

        public void commit(CommandId commandId, int sequence_number) {

            setup_connections();
            CommitRequest request = new CommitRequest {
                RequestId = commandId,
                SequenceNumber = sequence_number
            };

            foreach (BankPaxosServerConnection connection in _serverState.get_bank_servers().Values)
            {
                var client = connection.get_client();
                client.CommitAsync(request);
            }

        }

        public List<Task<CleanupReply>> cleanup (int last_commited, int slot) {

            setup_connections();
            CleanupRequest request = new CleanupRequest
            {
                LastApplied = last_commited,
                Slot = slot
            };

            List<Task<CleanupReply>> replies = new List<Task<CleanupReply>>();
            foreach (BankPaxosServerConnection connection in _serverState.get_bank_servers().Values) {
                var client = connection.get_client();
                var reply = client.CleanupAsync(request).ResponseAsync; 
                
                replies.Add(reply);
            }

            return replies;
        }
    }


    public class BankPaxosServerConnection
    {
        private string _url;
        private GrpcChannel _channel;
        private BankPaxos.BankPaxosClient _client;
        private FrozenInterceptor _interceptor;
        private bool setup = false;

        public BankPaxosServerConnection(string url) {
            this._url = url;
        }

        public void setup_stub(ServerState serverState) {
            if (!setup) {
                _interceptor = new FrozenInterceptor(serverState);
                _channel = GrpcChannel.ForAddress(_url);

                CallInvoker interceptingInvoker = _channel.Intercept(_interceptor);
                _client = new BankPaxos.BankPaxosClient(interceptingInvoker);

                setup = true;
            }
        }

        public void toggle_freeze() {

            if (_interceptor == null) {
                return;
            }

            _interceptor.toggle_freeze();
        }

        public BankPaxos.BankPaxosClient get_client() {
            return _client;
        }
    }
}
