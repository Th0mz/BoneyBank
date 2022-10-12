using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boney
{
    public class BoneyState {
        private const int timeslotsSize = 1024;
        private int[] timeslots = new int[timeslotsSize];
        private Paxos paxos;
        
        public BoneyState(Paxos paxos) { _paxos = paxos; }
        public int doCompareAndSwap(int slot, int leader)
        {
            lock (timeslots[slot]);
            if (timeslots[slot] != null) { return timeslots[slot]; }
            _paxos.runPaxosInstance(slot, leader);
            Monitor.Await(timeslots[slot]);
            return timeslots[slot];
        }    
    }
}
