using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OutpostOmega.Game;
using System.Threading;
using System.Diagnostics;

namespace OutpostOmega.Server
{
    /// <summary>
    /// Used to process the game and physics
    /// </summary>
    class MainGame
    {
        private Thread _mainThread;

        public World World { get; set; }
        public Network.Host Host { get; set; }
        public float Tickrate = 0;

        public MainGame(World world, Network.Host host)
        {
            this.World = world;
            this.Host = host;

            _mainThread = new Thread(Process);
            _mainThread.Name = "GameWorldProcessing";
            _mainThread.Start();
        }

        public bool _mainThreadAlive = true;

        private Stopwatch stopwatch = new Stopwatch();

        public void Stop()
        {
            _mainThreadAlive = false;
            Thread.Sleep(200);
            if (_mainThread.ThreadState != System.Threading.ThreadState.Stopped)
            {
                try
                {
                    _mainThread.Abort();
                }
                catch { }
            }
            World.PhysicSystem.CollisionSystem.threadManager.StopMe();
        }


        /*private void NetProcess()
        {
            while (_mainThreadAlive)
            {
                for (int i = 0; i < Host.ConnectedClients.Count; i++)
                    if (Host.ConnectedClients[i].Scope != null)
                    {
                        Host.ConnectedClients[i].Scope.FlushMessageQueue();
                    }
                Thread.Sleep(50);
            }
        }*/

        List<float> fpsStat = new List<float>();

        public bool Pause = false;

        public float FrameLimiter = 60;

        public float SecondCounter = 0;

        /// <summary>
        /// Processes the game
        /// </summary>
        private void Process()
        {
            float lastFrameTime = 0;
            while(_mainThreadAlive)
            {
                if (Pause)
                {
                    Thread.Sleep(500);
                    continue;
                }

                // Freeze the game when there are no player
                if (!BuildSettings.DevMode &&
                    HostSettings.Default.FreezeWhenEmpty &&
                    !(from client in Host.ConnectedClients
                     where client.Online
                     select client).Any())
                {
                    Thread.Sleep(500);
                    continue;
                }

                if (lastFrameTime > 0)
                {
                    float fps = 1000 / lastFrameTime;
                    if(fpsStat.Count > 200)
                        fpsStat.RemoveAt(0);
                    fpsStat.Add(fps);

                    float AverageFPS = 0;
                    foreach (float fpsS in fpsStat)
                        AverageFPS += fpsS;

                    AverageFPS /= fpsStat.Count;
                    Main.SetFPS(fps);
                    Tickrate = fps;
                }

                float step = (lastFrameTime / 1000);
                if (step == 0)
                    step = 0.001f;

                stopwatch.Reset();
                stopwatch.Start();


                if(this.World != null)
                {
                    bool Update = true;
                    /*if(HostSettings.Default.Heartbeat)
                        lock (Host.ConnectedClients)
                            for (int i = 0; i < Host.ConnectedClients.Count; i++)
                                if (!Host.ConnectedClients[i].Heartbeat)
                                    Update = false;*/

                    if (Update)
                    {
                        this.World.UpdateChunkRender();
                        this.World.Update(new OutpostOmega.Game.Tools.KeybeardState(), new OutpostOmega.Game.Tools.MouseState(), step);
                        //this.World.PhysicSystem.Step(step, true);

                        lock (Host.ConnectedClients)
                        {
                            for (int i = 0; i < Host.ConnectedClients.Count; i++)
                                if (Host.ConnectedClients[i].Scope != null && Host.ConnectedClients[i].Scope.NeedsUpdate)
                                {
                                    Host.ConnectedClients[i].Scope.Update();
                                    //client.Mind.Mob.Name = "Wischmoppp";
                                }
                        }
                    }
                }

                stopwatch.Stop();

                if (stopwatch.ElapsedMilliseconds < (1000 / FrameLimiter))
                {
                    float Difference = (1000 / FrameLimiter) - stopwatch.ElapsedMilliseconds;
                    Thread.Sleep((int)Difference);
                    lastFrameTime = Difference + stopwatch.ElapsedMilliseconds;

                }
                else
                    lastFrameTime = stopwatch.ElapsedMilliseconds;

                SecondCounter += stopwatch.ElapsedMilliseconds; 

                if(SecondCounter > 1000)
                {
                    SecondCounter = 0;
                }
            }
        }
    }
}
