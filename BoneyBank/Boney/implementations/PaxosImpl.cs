using Grpc.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Threading.Channels;
using Grpc.Net.Client;
using static System.Net.Mime.MediaTypeNames;

namespace Boney
{
    public class PaxosImpl : PaxosService.PaxosServiceBase {

        // lock this values 
        private int value = 0;

        private int last_proposal = 0;
        private int last_accepted = 0;

        private BoneyState state;

        public PaxosImpl (BoneyState _state) {
            state = _state;
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
        }


        // prepare funcionality
        public override Task<PrepareReply> Prepare
            (PrepareRequest request, ServerCallContext context) {

            return Task.FromResult(do_prepare(request));
        }

        private PrepareReply do_prepare(PrepareRequest request) {
            // accept code
            return new PrepareReply();
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



        // learn funcionality
        public override Task<LearnReply> Learn
            (LearnRequest request, ServerCallContext context) {

            return Task.FromResult(do_learn(request));
        }

        private LearnReply do_learn(LearnRequest request) {
            // accept code
            return new LearnReply();
        }


        /* keepalive funcionality
        public override Task<KeepaliveReply> Keepalive
            (KeepaliveRequest request, ServerCallContext context) {

            return Task.FromResult(do_keepalive(request));
        }

        private KeepaliveReply do_keepalive(KeepaliveRequest request) {
            // keepalive code
            return new KeepaliveReply();
        }
        */
    }
}