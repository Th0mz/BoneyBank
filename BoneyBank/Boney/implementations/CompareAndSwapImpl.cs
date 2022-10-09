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

        private Boney _boney;

        public CompareAndSwapImpl(Boney boney) {
            _boney = boney;
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
        }

        // compareAndSwap funcionality
        public override Task<CompareAndSwapReply> CompareAndSwap 
            (CompareAndSwapRequest request, ServerCallContext context) {

            return Task.FromResult(do_compareAndSwap(request));
        }

        private CompareAndSwapReply do_compareAndSwap(CompareAndSwapRequest request) {
            return new CompareAndSwapReply { 
                                Leader = _boney.doCompareAndSwap(request.slot, request.leader) };
        }
    }
}