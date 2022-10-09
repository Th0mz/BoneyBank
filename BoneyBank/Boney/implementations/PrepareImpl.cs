using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boney.implementations
{
    public class PrepareImpl : PrepareService.PrepareServiceBase
    {
        private Acceptor _acceptor;
        public PrepareImpl(Acceptor acceptor) { _acceptor = acceptor; }
    }
}
