using Grpc.Core;

namespace Boney
{
    public class PaxosImpl : PaxosService.PaxosServiceBase {
        // TODO : use read/write locks??? locks that permit
        // multiple reads if there is no one writting

        private const int _OK = 1;
        private const int _NOK = -1;

        private BoneyState state;
        private ServerState serverState;

        public PaxosImpl (BoneyState _state, ServerState _serverState) {
            state = _state;
            serverState = _serverState; 
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
        }


        // prepare funcionality
        public override Task<PrepareReply> Prepare
            (PrepareRequest request, ServerCallContext context) {

            return Task.FromResult(do_prepare(request));
        }

        private PrepareReply do_prepare(PrepareRequest request) {
            int slot = request.Slot;

            //Console.WriteLine("IMPL: PREPARE RECEIVED");
            lock (serverState.getPaxosSlot(slot)) {
                //Console.WriteLine("IMPL: GOT PAXOS SLOT LOCK");
                if (request.ProposalNumber > serverState.getLastPromisedSeqnum(slot)) {
                    //request.ProposalNumber
                    serverState.setLastPromisedSeqnum(slot, request.ProposalNumber);
                }
                

                //Console.WriteLine("IMPL: SENT PREPARE REPLY {VALUE="+ serverState.getLastAcceptedValue(slot) + " SEQNUM="+
                    //serverState.getLastAcceptedSeqnum(slot) + " PROMISE="+ serverState.getLastPromisedSeqnum(slot));
                return new PrepareReply
                {
                    LastAcceptedValue = serverState.getLastAcceptedValue(slot),
                    LastAcceptedSeqnum = serverState.getLastAcceptedSeqnum(slot),
                    LastPromisedSeqnum = serverState.getLastPromisedSeqnum(slot)
                };   
            }
        }


        // accept funcionality
        public override Task<AcceptReply> Accept
            (AcceptRequest request, ServerCallContext context) {

            return Task.FromResult(do_accept(request));
        }

        private AcceptReply do_accept(AcceptRequest request) {
            lock (serverState.getPaxosSlot(request.Slot)) {
                if (request.ProposalNumber == serverState.getLastPromisedSeqnum(request.Slot))
                {
                    serverState.setLastAcceptedSeqnum(request.Slot, request.ProposalNumber);
                    serverState.setLastAcceptedValue(request.Slot, request.Leader);

                    return new AcceptReply {
                        Status = ResponseCode.Ok
                    };
                }

                return new AcceptReply {
                    Status = ResponseCode.Nok
                };
            }

        }



        // learn funcionality
        public override Task<LearnReply> Learn
            (LearnRequest request, ServerCallContext context) {

            return Task.FromResult(do_learn(request));
        }

        private LearnReply do_learn(LearnRequest request) {
            // accept code
            int leader = request.Leader;
            int slot = request.Slot;           

            Slot slot_obj = state.get_slot(slot);
            lock (slot_obj) {
                if (slot_obj.has_leader()) {
                    return new LearnReply { };
                }
                slot_obj.set_leader(leader);
                Monitor.PulseAll(slot_obj);
            }

            return new LearnReply { };
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