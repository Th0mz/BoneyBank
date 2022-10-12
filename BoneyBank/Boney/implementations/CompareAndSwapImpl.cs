using Grpc.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Threading.Channels;
using Grpc.Net.Client;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.CompilerServices;

namespace Boney
{
    public class CompareAndSwapImpl : CompareAndSwapService.CompareAndSwapServiceBase {

        private BoneyState _state;

        public CompareAndSwapImpl(BoneyState state) {
            _state = state;
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
        }

        // compareAndSwap funcionality
        public override Task<CompareAndSwapReply> CompareAndSwap 
            (CompareAndSwapRequest request, ServerCallContext context) {

            return Task.FromResult(do_compareAndSwap(request));
        }

        private CompareAndSwapReply do_compareAndSwap(CompareAndSwapRequest request) {
            // compareAndSwap code

            /*
            
            lock (state.timeslots[slot]) 
                if state.timeslots[slot] != null
                    unlock
                    return state.timeslots[slot]
                
                paxosFrontend.prepare(leader, propose_number [id + offset])
                await state.timeslots[slot]
                return state.timeslots[slot]
            
            */

            Console.WriteLine(request.Leader + " " + request.Slot);
            Thread.Sleep(4000);
            return new CompareAndSwapReply { Leader = request.Leader};
        }
    }
}