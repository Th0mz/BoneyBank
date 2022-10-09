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

        private BoneyState state;
        private PaxosFrontend paxosClient;

        public CompareAndSwapImpl(BoneyState _state, PaxosFrontend _paxosClient) {
            state = _state;
            paxosClient = _paxosClient;
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
        }

        // compareAndSwap funcionality
        public override Task<CompareAndSwapReply> CompareAndSwap 
            (CompareAndSwapRequest request, ServerCallContext context) {

            return Task.FromResult(do_compareAndSwap(request));
        }

        private CompareAndSwapReply do_compareAndSwap(CompareAndSwapRequest request) {
            lock (state.timeslots[slot]);
            if (state.timeslots[slot] != null)
            {
                return state.timeslots[slot];
            }
            else
            {
                paxosClient.prepare();
                Monitor.Wait(state.timeslots[slot]);
            }
            
            return new CompareAndSwapReply { Leader = state.timeslots[slot] };
            // compareAndSwap code

            /*
            proposer :

            lock (state.timeslots[slot]) 
                if state.timeslots[slot] != null
                    unlock
                    return state.timeslots[slot]
                
                prepare(leader, propose_number [id + offset])
                await state.timeslots[slot]
                return state.timeslots[slot]
            
            */

            /*
            acceptor : 
            

            */

            /*
             learner : 
                
                state.timeslots[slot] = leader
                pulse all no lock
             */
        }
    }
}