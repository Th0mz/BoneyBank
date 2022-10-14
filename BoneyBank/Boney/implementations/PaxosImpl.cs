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
        private object mutex = new object();

        private int last_accepted_value = 0;
        private int last_promised_seqnum = 0;
        private int last_accepted_seqnum = 0;
        private int currentInstance = 1;

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
            // Console.WriteLine("[" + DateTime.Now.ToString("s.ffff") + "] " + "Acceptor (prepare) : begining of prepare");
            lock (mutex)
            {
                // Console.WriteLine("[" + DateTime.Now.ToString("s.ffff") + "] " + "Acceptor (prepare) : inside lock");
                if (request.Slot != currentInstance) {
                    
                    // Console.WriteLine("[" + DateTime.Now.ToString("s.ffff") + "] " + "Acceptor (prepare) : not current instance");
                    return new PrepareReply { 
                        CurrentInstance = false
                    };
                }

                if (request.ProposalNumber > last_promised_seqnum) {
                    //request.ProposalNumber
                    last_promised_seqnum = request.ProposalNumber;
                }

                // Console.WriteLine("[" + DateTime.Now.ToString("s.ffff") + "] " + "Acceptor (prepare) : returning");
                return new PrepareReply
                {
                    CurrentInstance = true,
                    LastAcceptedValue = last_accepted_value,
                    LastAcceptedSeqnum = last_accepted_seqnum,
                    LastPromisedSeqnum = last_promised_seqnum
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
            // Console.WriteLine("[" + DateTime.Now.ToString("s.ffff") + "] " + "Acceptor (accept) : begining of learn");
            lock (mutex)
            {
                // Console.WriteLine("[" + DateTime.Now.ToString("s.ffff") + "] " + "Acceptor (accept) : inside lock");


                if (request.Slot != currentInstance) {
                    // Console.WriteLine("[" + DateTime.Now.ToString("s.ffff") + "] " + "Acceptor (accept) : not current instance");
                    return new AcceptReply { CurrentInstance = false };
                }

                if (request.ProposalNumber == last_promised_seqnum)
                {
                    // Console.WriteLine("[" + DateTime.Now.ToString("s.ffff") + "] " + "Acceptor (accept) : accepted");
                    last_accepted_seqnum = last_promised_seqnum;
                    last_accepted_value = request.Leader;

                    return new AcceptReply {
                        Status = _OK,
                        CurrentInstance = true
                    };
                }

                // Console.WriteLine("[" + DateTime.Now.ToString("s.ffff") + "] " + "Acceptor (accept) : not accepted");
                return new AcceptReply {
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

            // Console.WriteLine("[" + DateTime.Now.ToString("s.ffff") + "] " + "Learner : begining of accept");
            lock (mutex)
            {
                // Console.WriteLine("[" + DateTime.Now.ToString("s.ffff") + "] " + "Learner : inside lock");
                if (slot != currentInstance) {
                    // Console.WriteLine("[" + DateTime.Now.ToString("s.ffff") + "] " + "Learner : not current instance");
                    return new LearnReply { };
                }

                //erase last paxos instance's variables
                last_accepted_value = 0;
                last_promised_seqnum = 0;
                last_accepted_seqnum = 0;
                currentInstance++;
                
                Slot slot_obj = state.get_slot(slot);
                // Console.WriteLine("[" + DateTime.Now.ToString("s.ffff") + "] " + "Learner : got slot object");
                
                lock (slot_obj)
                {
                    // Console.WriteLine("[" + DateTime.Now.ToString("s.ffff") + "] " + "Learner : inside slot_obj lock");
                    if (slot_obj.has_leader()) {
                        // Console.WriteLine("[" + DateTime.Now.ToString("s.ffff") + "] " + "Learner : leader already elected");
                        return new LearnReply { };
                    }

                    slot_obj.set_leader(leader);
                    // Console.WriteLine("[" + DateTime.Now.ToString("s.ffff") + "] " + "Learner : pulsing all locked processes");
                    
                    Monitor.PulseAll(slot_obj);
                }

                return new LearnReply { };
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