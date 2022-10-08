using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boney
{
    internal class BoneyState {
        private const int timeslotsSize = 1024;
        private int[] timeslots = new int[timeslotsSize];

        private PaxosService.PaxosServiceClient[] boneyServers = new PaxosService.PaxosServiceClient[3];
        private int coordinator;
        private int id;
        // timer que a cada delta segundos chama handler??


    }
}
