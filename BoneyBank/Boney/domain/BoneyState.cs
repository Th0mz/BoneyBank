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
        private bool _proposed = false;

        public bool has_leader() {
            return _leader != no_leader;
        }

        public bool has_proposed () {
            return _proposed;
        }

        public void set_proposed () {
            _proposed = true;
        }

        public int get_leader() {
            return _leader;
        }

        public void set_leader(int leader) {
            _leader = leader;
        }
    }

    public class BoneyState {

        // TODO : remove static size => change this to a list 
        private int _size;
        private Slot[] timeslots;

        public BoneyState (int size) {
            _size = size + 1;
            timeslots = new Slot[_size];
            
            for (int i = 0; i < _size; i++) {
                timeslots[i] = new Slot();
            }
        }

        public Slot get_slot(int index) {
            if (index < timeslots.Length) {
                return timeslots[index];
            }

            return null;
        }
        
    }
}
