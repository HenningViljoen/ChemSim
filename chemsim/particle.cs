using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chemsim
{
    [Serializable]
    public class particle //A class that will store the details and fitness of one particle for the PSO
    {
        public List<double> currentmvs; //Manipulated Variables in the particle - current.
        public List<double> bestmvs; //Manipulated Variables in the particle - best solution in history of current objective.
        public List<double> currentspeed; //Speed of each particles along each MV dimension.

        public double currentfitness;
        public double bestfitness;
        public double maxvalueformv;

        public particle(int nrmvs, Random rand, double amaxvalueformv)
        {
            currentmvs = new List<double>();
            bestmvs = new List<double>();
            currentspeed = new List<double>();
            maxvalueformv = amaxvalueformv;
            for (int i = 0; i < nrmvs; i++)
            {
                currentmvs.Add(0);
                bestmvs.Add(0);
                currentspeed.Add(0);
            }
            currentfitness = Double.MaxValue;
            bestfitness = Double.MaxValue;
            initrandom(rand);
        }

        public void initrandom(Random rand)
        {
            for (int i = 0; i < currentmvs.Count; i++)
            {
                //currentmvs[i] = rand.Next(0, global.DefaultMaxValueforMVs + 1);
                currentmvs[i] = rand.NextDouble() * maxvalueformv;
                //bestmvs[i] = rand.Next(0, global.DefaultMaxValueforMVs + 1);
            }
        }

        public void copyfrom(particle particlecopyfrom)
        {
            for (int i = 0; i < currentmvs.Count; i++)
            {
                currentmvs[i] = particlecopyfrom.currentmvs[i];
                bestmvs[i] = particlecopyfrom.bestmvs[i];
            }
        }

    }
}
