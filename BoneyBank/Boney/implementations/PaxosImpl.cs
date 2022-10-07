using Grpc.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Threading.Channels;
using Grpc.Net.Client;
using static System.Net.Mime.MediaTypeNames;

public class PaxosImpl : PaxosService.PaxosServiceBase {

    public PaxosImpl() {
        AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
    }

    // accept funcionality
    public override Task<AcceptReply> Accept
        (AcceptRequest request, ServerCallContext context) {

        return Task.FromResult(do_accept(request));
    }

    private AcceptReply do_accept(AcceptRequest request) {
        // accept code
        return new AcceptReply();
    }


    // keepalive funcionality
    public override Task<KeepaliveReply> Keepalive
        (KeepaliveRequest request, ServerCallContext context) {

        return Task.FromResult(do_keepalive(request));
    }

    private KeepaliveReply do_keepalive(KeepaliveRequest request) {
        // keepalive code
        return new KeepaliveReply();
    }
}