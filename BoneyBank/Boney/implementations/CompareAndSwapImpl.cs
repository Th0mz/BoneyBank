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
            int leader = request.Leader;
            int slot = request.Slot;
            bool is_coordinator;
            bool has_proposed;

            // DEBUG
            Slot slot_obj = _state.get_slot(slot);


            lock (slot_obj)
            {
                if (slot_obj.has_leader()) {
                    return new CompareAndSwapReply
                    {
                        Leader = slot_obj.get_leader()
                    };
                }
                is_coordinator = _serverState.is_coordinator();
                has_proposed = slot_obj.has_proposed();
                slot_obj.set_proposed();
            }

            //Only propose if believes to be coordinator and hasnt proposed a value yet
            if (is_coordinator && !has_proposed) {
                _paxosFrontend.propose(slot, leader);
            }
            
            lock (slot_obj) {
                while (!slot_obj.has_leader()) {
                    Monitor.Wait(slot_obj);
                }
                leader = slot_obj.get_leader();
            }

            return new CompareAndSwapReply {
                Leader = leader
            };

        }
    }
}