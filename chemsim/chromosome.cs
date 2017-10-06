using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chemsim
{
    [Serializable]
    public class chromosome //A class that will store the details and fitness of one chromosome for the GA
    {
        public List<int> mvs; //Manipulated Variables in the chromosome.  For now the mvs will be numbers that are integer percentages (0 - 100) of the 
                                        //EU range of the variables.
        public double fitness;

        public chromosome(int nrmvs, Random rand)
        {
            mvs = new List<int>();
            for (int i = 0; i < nrmvs; i++)
            {
                mvs.Add(0);
            }
            fitness = 0;
            initrandom(rand);
        }

        public void initrandom(Random rand)
        {
            for (int i = 0; i < mvs.Count; i++)
            {
                mvs[i] = rand.Next(0, global.DefaultMaxValueforMVs + 1);
            }
        }

        public void copyfrom(chromosome chromosomecopyfrom)
        {
            for (int i = 0; i < mvs.Count; i++)
            {
                mvs[i] = chromosomecopyfrom.mvs[i];
            }
        }
    }
}
