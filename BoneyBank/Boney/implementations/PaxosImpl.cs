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
        private int last_accepted_value = 0;
        private object lockAcceptedValue = new object();

        private int last_promised_seqnum = 0;
        private object lockPromisedSeqnum = new object();

        private int last_accepted_seqnum = 0;
        private object lockAcceptedSeqnum = new object();

        private int currentInstance = 1;
        private object lockCurrentInstance = new object();

        private const int _OK = 1;
        private const int _NOK = -1;

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
            lock (lockAcceptedValue) lock(lockPromisedSeqnum) lock(lockAcceptedSeqnum) lock(lockCurrentInstance)
            {
                if (request.Slot != currentInstance) return new PrepareReply { CurrentInstance = false };

                if (request.ProposalNumber > last_promised_seqnum) {
                    //request.ProposalNumber
                    last_promised_seqnum = request.ProposalNumber;
                }

                return new PrepareReply
                {
                    LastAcceptedValue = last_accepted_value,
                    LastAcceptedSeqnum = last_accepted_seqnum,
                    LastPromisedSeqnum = last_promised_seqnum,
                    CurrentInstance = true
                };   
            }
            

        }


        // accept funcionality
        public override Task<AcceptReply> Accept
            (AcceptRequest request, ServerCallContext context) {

            return Task.FromResult(do_accept(request));
        }

        private AcceptReply do_accept(AcceptRequest request) {
            //TODO tommy : synchronize :D (place locks in every shared variable)

            lock (lockAcceptedValue) lock (lockPromisedSeqnum) lock (lockAcceptedSeqnum) lock (lockCurrentInstance)
            {
                if (request.Slot != currentInstance) return new AcceptReply { CurrentInstance = false };

                if (request.ProposalNumber == last_promised_seqnum)
                {
                    last_accepted_seqnum = last_promised_seqnum;
                    last_accepted_value = request.Leader;

                    return new AcceptReply
                    {
                        Status = _OK,
                        CurrentInstance = true
                    };
                }

                return new AcceptReply
                {
                    Status = _NOK,
                    CurrentInstance = true
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

            lock (lockAcceptedValue) lock (lockPromisedSeqnum) lock (lockAcceptedSeqnum) lock (lockCurrentInstance)
            {
                if (slot != currentInstance) return new LearnReply { };

                //erase last paxos instance's variables
                last_accepted_value = 0;
                last_promised_seqnum = 0;
                last_accepted_seqnum = 0;
                currentInstance++;

                Slot slot_obj = state.get_slot(slot);
                lock (slot_obj)
                {
                    if (slot_obj.has_leader())
                    {
                        return new LearnReply { };
                    }

                    slot_obj.set_leader(leader);

                    Monitor.PulseAll(slot_obj);

                    return new LearnReply { };
                }
            }
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