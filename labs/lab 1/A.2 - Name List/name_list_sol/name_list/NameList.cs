using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace name_list
{
    internal class NameList : INameList
    {
        List<string> names = new List<string>();
        public void addName (string name)
        {
            names.Add(name);
        }

        public string[] getNames()
        {
            return names.ToArray();
        }

        public void eraseNames()
        {
            names = new List<string>();
        }
    }
}
