using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Boney
{
    class Boney {

        static void Main(string[] args) {
 

            BoneyState _state = new BoneyState();

            Proposer _proposer = new Proposer(this);
            Acceptor _acceptor = new Acceptor(this);
            Learner _learner = new Learner(this);

            // paxos frontend vai abrir conexões com todos os servers boney
            // ou seja primeiro deve ser comunicada a lista de servidores
            //PaxosFrontend paxosClient = new PaxosFrontend(this);

            Server server = new Server
            {
                Services = { CompareAndSwapService.BindService(new CompareAndSwapImpl(this)),
                             PaxosService.BindService(new PaxosImpl(_state)),
                             PrepareService.BindService(new PrepareImpl(_acceptor)),
                             AcceptService.BindService(new AcceptImpl(_acceptor)),
                             LearnService.BindService(new LearnImpl(_learner))},
                Ports = { new ServerPort("localhost", 5001, ServerCredentials.Insecure) }
            };

            server.Start();

            Console.WriteLine("ChatServer server listening on port " + 5001);
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();

            server.ShutdownAsync().Wait();
        }

        public void addServer(int id, string url){ _proposer.addServer(id, url); }

        public int doCompareAndSwap(int slot, int leader)
        {
            lock (_state.timeslots[slot]);
            if (_state.timeslots[slot] != null)
            {
                return _state.timeslots[slot];
            }

            _proposer.runPaxosInstance(slot, leader);
            Monitor.Await(_state.timeslots[slot]);
            return _state.timeslots[slot];

        }
    }
}