using Grpc.Core;

namespace Boney
{
    public class CompareAndSwapImpl : CompareAndSwapService.CompareAndSwapServiceBase {

        private BoneyState _state;
        private PaxosFrontend _paxosFrontend;

        public CompareAndSwapImpl(BoneyState state, PaxosFrontend frontend) {
            _state = state;
            _paxosFrontend = frontend;
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
        }

        // compareAndSwap funcionality
        public override Task<CompareAndSwapReply> CompareAndSwap 
            (CompareAndSwapRequest request, ServerCallContext context) {

            return Task.FromResult(do_compareAndSwap(request));
        }

        private CompareAndSwapReply do_compareAndSwap(CompareAndSwapRequest request) {
            // compareAndSwap code

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
            // TODO : coordinator
            // if (coordinator) {
            _paxosFrontend.propose(slot, leader);
            //Console.WriteLine("CompareAndSwap : Wait");
            // }

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