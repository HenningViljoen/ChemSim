using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chemsim
{
    [Serializable]
    public class molecules
    {
        public List<molecule> molecularlist;

        public molecules()
        {
            molecularlist = new List<molecule>();
        }

        public molecule match(string aname)
        {
            molecule m = null;
            for (int i = 0; i < molecularlist.Count; i++)
            {
                if (molecularlist[i].name == aname) { m = molecularlist[i]; }
            }
            return m;

        }
    }
}