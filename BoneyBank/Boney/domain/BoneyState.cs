using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boney
{
    public class BoneyState {
        //PAXOS SPECIFIC
        // hardcoded max number of processes
        private static int max = 3;
        private Dictionary<int, PaxosService.PaxosServiceClient> boneyServers = 
            new Dictionary<int, PaxosService.PaxosServiceClient>();
        private int _lastSeqNumber;
        private int _lastVotedLeader;

        //SLOTS
        private const int timeslotsSize = 1024;
        internal int[] timeslots = new int[timeslotsSize];

        private int _id;

        public BoneyState(int id)
        {
            _id = id;
        }

        public int getId() { return _id; }
        public int getLastSeqNumber() { return _lastSeqNumber; }
        public int getLastVotedLeader() { return _lastVotedLeader; }
        public static int getNumberProcesses() { return max; }
        public Dictionary<int, PaxosService.PaxosServiceClient> getBoneyServers() { return boneyServers; }

        public void setLastSeqNumber(int lastSeqNumber) { _lastSeqNumber = lastSeqNumber; }
        public void setLastVotedLeader(int lastVotedLeader) { _lastVotedLeader = lastVotedLeader; }

        public void add_server (int id, string url) {
            GrpcChannel channel = GrpcChannel.ForAddress(url);
            PaxosService.PaxosServiceClient client = new PaxosService.PaxosServiceClient(channel);

            boneyServers.Add(id, client);
        }
        
    }
}
