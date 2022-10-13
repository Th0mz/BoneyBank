using Grpc.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Threading.Channels;
using Grpc.Net.Client;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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
            Console.WriteLine("CompareAndSwap(" + leader + ", " + slot + ")");

            Slot slot_obj = _state.get_slot(slot);
            lock (slot_obj)
            {
                if (slot_obj.has_leader())
                {
                    return new CompareAndSwapReply
                    {
                        Leader = slot_obj.get_leader()
                    };
                }

                // DEBUG
                Console.WriteLine("CompareAndSwap : Proposing leader");
                // TODO : coordinator
                // if (coordinator) {
                // TODO : testar se for escolhido um valor a meio do propose se isto funciona
                _paxosFrontend.propose(slot, leader);
                Console.WriteLine("CompareAndSwap : Wait");
                // }
                Monitor.Wait(slot_obj);

                return new CompareAndSwapReply {
                    Leader = slot_obj.get_leader()
                };

            }

        }
    }
}