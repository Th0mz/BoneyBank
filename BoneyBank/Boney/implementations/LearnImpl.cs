using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boney.implementations
{
    public class LearnImpl : LearnService.LearnServiceBase
    {
        private Learner _learner;
        public LearnImpl(Learner learner) { _learner = learner; }
    }
}
