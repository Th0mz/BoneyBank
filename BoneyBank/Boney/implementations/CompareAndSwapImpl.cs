using Grpc.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Threading.Channels;
using Grpc.Net.Client;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.CompilerServices;

public class CompareAndSwapImpl : CompareAndSwapService.CompareAndSwapServiceBase {

    public CompareAndSwapImpl() {
        AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
    }

    // compareAndSwap funcionality
    public override Task<CompareAndSwapReply> CompareAndSwap 
        (CompareAndSwapRequest request, ServerCallContext context) {

        return Task.FromResult(do_compareAndSwap(request));
    }

    private CompareAndSwapReply do_compareAndSwap(CompareAndSwapRequest request) {
        return new CompareAndSwapReply();
    }
}