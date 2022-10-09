using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boney.implementations
{
    public class AcceptImpl : AcceptService.AcceptServiceBase
    {
        private Acceptor _acceptor;
        public AcceptImpl(Acceptor acceptor) { _acceptor = acceptor; }
    }
}
