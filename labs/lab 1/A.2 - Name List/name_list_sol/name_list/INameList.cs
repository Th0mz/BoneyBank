using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace name_list
{
    internal interface INameList
    {
        void addName(string name);
        string[] getNames();
        void eraseNames();
    }
}
