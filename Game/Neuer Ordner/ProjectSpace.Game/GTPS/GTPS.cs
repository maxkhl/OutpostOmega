using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using OutpostOmega.Game.turf;

namespace OutpostOmega.Game.GTPS
{
    /// <summary>
    /// Gas Temperatur and Pressure Simulation
    /// </summary>
    public class GTPS : IDisposable
    {
        /// <summary>
        /// Determins the amount of gas that gets transfered per tick
        /// </summary>
        private const float TickFactor = 0.1f;

        /// <summary>
        /// World, this GTPS-instance is working with
        /// </summary>
        public World World { get; protected set; }

        /// <summary>
        /// Main thread for simulation processing
        /// </summary>
        public Thread MainThread { get; set; }

        /// <summary>
        /// Is the simulation running?
        /// </summary>
        public bool Running { get; protected set; }

        /// <summary>
        /// Can be used to simulate heavy load on the mainthread. Will add a timout (in MS) to every tick
        /// </summary>
        public int SimulateLoad { get; set; }

        public GTPS(World World)
        {
            this.World = World;

            SimulateLoad = 1000;

            this.MainThread = new Thread(Work);
        }

        /// <summary>
        /// Starts the simulation processing
        /// </summary>
        public void Start()
        {
            if(!this.Running)
            {
                this.Running = true;
                this.MainThread.Start();
            }
        }

        /// <summary>
        /// Stops the simulation processing
        /// Stop-progress is running asynch. Simulation will be stopped after the current tick
        /// </summary>
        public void Stop()
        {
            if(this.Running)
            {
                this.Running = false;
            }
        }

        /// <summary>
        /// Work method for the mainThread
        /// </summary>
        private void Work()
        {
            while(this.Running)
            {

                // Iterate through each structure in our world
                for(int s = 0; s < World.Structures.Count; s++)
                {
                    var structure = World.Structures[s];

                    // Iterate through each chunk
                    for(int c = 0; c < structure.chunks.Count; c++)
                    {
                        var chunk = structure.chunks[c];
                        
                        // Iterate through each block
                        for(int x = 0; x < turf.Chunk.SizeXYZ; x++)
                            for(int y = 0; y < turf.Chunk.SizeXYZ; y++)
                                for(int z = 0; z < turf.Chunk.SizeXYZ; z++)
                                {
                                    var block = chunk.blocks[x, y, z];

                                    if(block.NeedsProcessing)
                                    {
                                        if (!Block.CanContainGas(block)) // Clean up errors
                                        {
                                            block.NeedsProcessing = false;
                                            continue;
                                        }

                                        int absX = (int)chunk.Position.X + x,
                                            absY = (int)chunk.Position.Y + y,
                                            absZ = (int)chunk.Position.Z + z;


                                        List<Block> Neighours = new List<Block>();

                                        // X-line
                                        for(int nX = absX - 1; nX < absX + 1; nX += 2)
                                        {
                                            Neighours.Add(structure[nX, absY, absZ]);
                                        }

                                        // Y-line
                                        for (int nY = absY - 1; nY < absY + 1; nY += 2)
                                        {
                                            Neighours.Add(structure[absX, nY, absZ]);
                                        }

                                        // Z-line
                                        for (int nZ = absZ - 1; nZ < absZ + 1; nZ += 2)
                                        {
                                            Neighours.Add(structure[absX, absY, nZ]);
                                        }
                                        
                                        // Check for processable neighbours
                                        for (int n = 0; n < Neighours.Count; n++)
                                        {
                                            if (!Block.CanContainGas(Neighours[n]))
                                                Neighours.RemoveAt(n);
                                        }

                                        //Sort neighbours for pressure
                                        Neighours.Sort(delegate(Block a, Block b)
                                        {
                                            int aPressure = Block.Pressure(a),
                                                bPressure = Block.Pressure(b);

                                            if (aPressure > bPressure) return 1;
                                            else if (aPressure == bPressure) return 0;
                                            else if (aPressure > bPressure) return -1;
                                            else return 0;
                                        });

                                        chunk.AtmosActive = true;

                                        for (int g = 0; g < block.gasComposition.Count; g++)
                                        {
                                            var gas = block.gasComposition[g];

                                            // Iterate through neighbours
                                            for (int n = 0; n < Neighours.Count; n++)
                                            {
                                                var neighbour = Neighours[n];
                                                float PressureFactor = 1; //how fast the gas will flow
                                                float SurfaceFactor = (float)Neighours.Count / 8; //how spread/concentrated the exchange is

                                                float Amount = (float)gas.Units * SurfaceFactor * PressureFactor * TickFactor; //Units to exchange between this turfs

                                                if (Amount > gas.Units) Amount = gas.Units;

                                                gas.Units -= Amount; //Remove gas
                                                Block.ModGas(ref neighbour, gas.GasID, Amount); //Add gas
                                            }
                                            
                                            block.gasComposition[g] = gas;
                                        }
                                        chunk.blocks[x, y, z] = block;
                                    }
                                }
                    }
                }

                Thread.Sleep(1 + SimulateLoad);
            }
        }

        /// <summary>
        /// Determins if object is currently in a disposal process
        /// </summary>
        public bool Disposing { get; set; }
        public void Dispose()
        {
            Disposing = true;

            //Tell thread to stop
            this.Running = false;

            //Wait 3 seconds if necessary
            int Timeout = 3000;
            while(MainThread.IsAlive && Timeout > 0)
            {
                Timeout -= 50;
                Thread.Sleep(50);
            }

            //Kill if still running
            if (MainThread.IsAlive)
                try
                {
                    MainThread.Abort();
                }
                catch { }
        }

    }
}
