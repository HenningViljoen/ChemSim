using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace chemsim
{
    [Serializable]
    public class simulation : IDisposable
    {
        public List<unitop> unitops; //All the unit ops in the model.
        public List<stream> streams; //All the items streams in the model.
        public List<block> blocks; //The DCS blocks in the simulation.
        public List<signal> signals; //All the signals in the model.
        public List<pidcontroller> pidcontrollers; //All pidcontrollers in the model.
        //public List<controlmvsignalsplitter> controlmvsignalsplitters;
        public List<nmpc> nmpccontrollers; //All NMPC controllers in the model.
        public int simi; //Counting index for this class for simulation indexing for historisation and simulation.

        public bool simulating;

        public simtimer simtime;

        public simulation()
        {
            initsimulation();
        }

        public simulation(simulation simcopyfrom)
        {
            initsimulation();
            copyfrom(simcopyfrom);
        }

        private void initsimulation()
        {
            unitops = new List<unitop>();
            streams = new List<stream>();
            signals = new List<signal>();
            pidcontrollers = new List<pidcontroller>();
            blocks = new List<block>();
            nmpccontrollers = new List<nmpc>();

            simtime = new simtimer(0, 0, 0, 0);
            setsimulationready();
        }

        public void copyfrom(simulation simcopyfrom)
        {
            //MemoryStream myStream = new MemoryStream();
            //simulation tempsim = new simulation();
            //BinaryFormatter bf = new BinaryFormatter();
            //bf.Serialize(myStream, simcopyfrom);
            
            //myStream.Seek(0, SeekOrigin.Begin);
            
            //tempsim = (simulation)bf.Deserialize(myStream);
            //myStream.Close();

            //unitops = tempsim.unitops;
            //streams = tempsim.streams;
            //pidcontrollers = tempsim.pidcontrollers;

            if (unitops.Count == 0)
            {
                unitops.Clear();
                for (int i = 0; i < simcopyfrom.unitops.Count; i++) 
                {
                    
                    switch (simcopyfrom.unitops[i].objecttype)
                    {
                        case objecttypes.CoolingTowerSimple:
                            unitops.Add(new coolingtowersimple(simcopyfrom.unitops[i]));
                            break;
                        case objecttypes.CoolingTowerHeatExchangerSimple:
                            unitops.Add(new coolingtowerheatexchangersimple(simcopyfrom.unitops[i]));
                            break;
                        case objecttypes.CoolingTower:
                            unitops.Add(new coolingtower(simcopyfrom.unitops[i]));
                            break;
                        case objecttypes.DistillationColumn:
                            unitops.Add(new distillationcolumn(simcopyfrom.unitops[i]));
                            break;
                        case objecttypes.Flange:
                            unitops.Add(new flange(simcopyfrom.unitops[i]));
                            break;
                        case objecttypes.GasPipe:
                            unitops.Add(new gaspipe(simcopyfrom.unitops[i]));
                            break;
                        case objecttypes.HeatExchangerSimple:
                            unitops.Add(new heatexchangersimple(simcopyfrom.unitops[i]));
                            break;
                        case objecttypes.Mixer:
                            unitops.Add(new mixer(simcopyfrom.unitops[i]));
                            break;
                        case objecttypes.Pump:
                            unitops.Add(new pump(simcopyfrom.unitops[i]));
                            break;
                        case objecttypes.Tee:
                            unitops.Add(new tee(simcopyfrom.unitops[i]));
                            break;
                        case objecttypes.Valve:
                            unitops.Add(new valve(simcopyfrom.unitops[i]));
                            break;
                        case objecttypes.Tank:
                            unitops.Add(new tank(simcopyfrom.unitops[i]));
                            break;
                        default:
                            break;
                    }

                    
                }
            }
            else
            {
                for (int i = 0; i < simcopyfrom.unitops.Count; i++) 
                { 
                    unitops[i].copyfrom(simcopyfrom.unitops[i]); 
                }
            }

            if (streams.Count == 0)
            {
                streams.Clear();
                for (int i = 0; i < simcopyfrom.streams.Count; i++) { streams.Add(new stream(simcopyfrom.streams[i])); }
            }
            else
            {
                for (int i = 0; i < simcopyfrom.streams.Count; i++) { streams[i].copyfrom(simcopyfrom.streams[i]); }
            }

            if (signals.Count == 0)
            {
                signals.Clear();
                for (int i = 0; i < simcopyfrom.signals.Count; i++) { signals.Add(new signal(simcopyfrom.signals[i])); }
            }
            else
            {
                for (int i = 0; i < simcopyfrom.signals.Count; i++) { streams[i].copyfrom(simcopyfrom.signals[i]); }
            }

            if (pidcontrollers.Count == 0)
            {
                pidcontrollers.Clear();
                for (int i = 0; i < simcopyfrom.pidcontrollers.Count; i++) { pidcontrollers.Add(new pidcontroller(simcopyfrom.pidcontrollers[i])); }
            }
            else
            {
                for (int i = 0; i < simcopyfrom.pidcontrollers.Count; i++) { pidcontrollers[i].copyfrom(simcopyfrom.pidcontrollers[i]); }
            }

            if (blocks.Count == 0)
            {
                blocks.Clear();
                for (int i = 0; i < simcopyfrom.blocks.Count; i++)
                {

                    switch (simcopyfrom.blocks[i].objecttype)
                    {
                        case objecttypes.ControlMVSignalSplitter:
                            blocks.Add(new controlmvsignalsplitter(simcopyfrom.blocks[i]));
                            break;
                        
                        default:
                            break;
                    }


                }
               
            }
            else
            {
                for (int i = 0; i < simcopyfrom.blocks.Count; i++) { blocks[i].copyfrom(simcopyfrom.blocks[i]); }
            }

            //public List<nmpc> nmpccontrollers; The nmpc controller(s) are not going to be copied at this point in time.
            simi = simcopyfrom.simi; //Counting index for this class for simulation indexing for historisation and simulation.
        }

        public void setsimulationready()
        {
            simi = 0;
            setsimulating(false);
            simtime.reset();
        }

        public void setsimulating(bool asimulating)
        {
            simulating = asimulating;
        }

        public void Dispose()
        {
        }

        public void drawnetwork(Graphics G)
        {
            G.Clear(Color.White);
            for (int i = 0; i < unitops.Count; i++) { unitops[i].draw(G); }
            for (int i = 0; i < streams.Count; i++) { streams[i].draw(G); }
            for (int i = 0; i < signals.Count; i++) { signals[i].draw(G); }
            for (int i = 0; i < pidcontrollers.Count; i++) { pidcontrollers[i].draw(G); }
            if (blocks != null) { for (int i = 0; i < blocks.Count; i++) { blocks[i].draw(G); } }
            for (int i = 0; i < nmpccontrollers.Count; i++) { nmpccontrollers[i].draw(G); }
        }

        public void simulateplant(bool historise) //This method is called from main simulate method, and also from the nmpc controller.
        {
            for (int i = 0; i < unitops.Count; i++) { unitops[i].update(simi, historise); }
            for (int i = 0; i < streams.Count; i++) { streams[i].update(simi, historise); }
            for (int i = 0; i < signals.Count; i++) { signals[i].update(simi, historise); }
            for (int i = 0; i < pidcontrollers.Count; i++) { pidcontrollers[i].update(simi, historise); }
            if (blocks != null) { for (int i = 0; i < blocks.Count; i++) { blocks[i].update(simi, historise); } }
        }
        
        public void simulate(Graphics G, List<Form> detailtrends)
        {
            if (nmpccontrollers == null) { nmpccontrollers = new List<nmpc>(); } //THIS LINE SHOULD AT SOME POINT BE DELETED WHEN THE MODEL FILE HAS 
                                                                                    //BEEN RECREATED WITH THE LATEST VERSION OF THIS CLASS.
            if (blocks == null) { blocks = new List<block>(); }
            for (int i = 0; i < nmpccontrollers.Count; i++) { nmpccontrollers[i].update(simi, true); }
            
            simulateplant(true);

            if ((detailtrends != null) && (simi % global.TrendUpdateIterPeriod == 0))
            {
                for (int i = 0; i < detailtrends.Count; i++)
                {
                    if (detailtrends[i] != null && detailtrends[i].Visible)
                    {
                        detailtrends[i].Invalidate();
                        detailtrends[i].Update();
                    }
                }
            }


            simtime.tick();
            simi++;
        }


    }
}
