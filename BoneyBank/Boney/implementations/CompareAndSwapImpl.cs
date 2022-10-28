using Grpc.Core;

namespace Boney
{
    public class CompareAndSwapImpl : CompareAndSwapService.CompareAndSwapServiceBase {

        private BoneyState _state;
        private PaxosFrontend _paxosFrontend;
        private ServerState _serverState;

        public CompareAndSwapImpl(BoneyState state, PaxosFrontend frontend, ServerState serverState) {
            _state = state;
            _paxosFrontend = frontend;
            _serverState = serverState;
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
        }

        // compareAndSwap funcionality
        public override Task<CompareAndSwapReply> CompareAndSwap 
            (CompareAndSwapRequest request, ServerCallContext context) {

            return Task.FromResult(do_compareAndSwap(request));
        }

        private CompareAndSwapReply do_compareAndSwap(CompareAndSwapRequest request) {
            // compareAndSwap code
            //THREAD _serverState.addThread(Thread.CurrentThread);
            int leader = request.Leader;
            int slot = request.Slot;

            // DEBUG
            // Console.WriteLine("[" + DateTime.Now.ToString("s.ffff") + "] " + "CompareAndSwap(" + leader + ", " + slot + ")");
            Slot slot_obj = _state.get_slot(slot);

            lock (slot_obj)
            {
                if (slot_obj.has_leader())
                {
                    // Console.WriteLine("[" + DateTime.Now.ToString("s.ffff") + "] " + "CompareAndSwap : Leader already exists");
                    return new CompareAndSwapReply
                    {
                        Leader = slot_obj.get_leader()
                    };
                }
            }

            // DEBUG
            // Console.WriteLine("[" + DateTime.Now.ToString("s.ffff") + "] " + "CompareAndSwap : Proposing leader");
            
            // TODO : need to syncronize acess to server state
            if (_serverState.is_coordinator()) {
                _paxosFrontend.propose(slot, leader);
            }

            lock (slot_obj)
            {
                while (!slot_obj.has_leader()) {
                    Monitor.Wait(slot_obj);
                }
                leader = slot_obj.get_leader();
            }


            // Console.WriteLine("[" + DateTime.Now.ToString("s.ffff") + "] " + "CompareAndSwap : chosen value " + slot_obj.get_leader());
            return new CompareAndSwapReply {
                Leader = leader
            };

        }
    }
}