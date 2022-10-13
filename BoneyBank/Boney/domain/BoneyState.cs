using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Boney
{
    public class Slot {
        private static int no_leader = -1;
        private int _leader = no_leader;

        public bool has_leader() {
            return _leader != no_leader;
        }

        public int get_leader() {
            return _leader;
        }

        public void set_leader(int leader) {
            _leader = leader;
        }
    }
    public class BoneyState {

        // TODO : remove static size
        private static int size = 1024;
        private Slot[] timeslots = new Slot[size];

        public BoneyState () {
            for (int i = 0; i < size; i++) {
                timeslots[i] = new Slot();
            }
        }

        public Slot get_slot(int index) {
            if (index < size) {
                return timeslots[index];
            }

            return null;
        }
        
    }
}
