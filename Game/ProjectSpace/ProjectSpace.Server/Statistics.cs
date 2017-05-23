using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutpostOmega.Server
{
    class Statistics
    {
        public Dictionary<string, List<Dataset>> Data;

        public double StartTime;
        public double DumpTime = 0;

        private bool Ready = false;

        Main MainForm;
        Task UpdateTask;
        public Statistics(Main MainForm)
        {
            this.MainForm = MainForm;
            StartTime = Environment.TickCount;

            Data = new Dictionary<string, List<Dataset>>();
            Data["Connections"] = new List<Dataset>();

            Data["ReceivedBytes"] = new List<Dataset>();
            Data["ReceivedMessages"] = new List<Dataset>();
            Data["ReceivedPackets"] = new List<Dataset>();

            Data["SentBytes"] = new List<Dataset>();
            Data["SentMessages"] = new List<Dataset>();
            Data["SentPackets"] = new List<Dataset>();

            Data["StorageBytesAllocated"] = new List<Dataset>();
            Data["BytesInRecyclePool"] = new List<Dataset>();

            Data["GameObjectCount"] = new List<Dataset>();
            Data["ConnectedClients"] = new List<Dataset>();

            Data["Tickrate"] = new List<Dataset>();

            Ready = true;

            this.UpdateTask = new Task(new Action(Update));
            this.UpdateTask.Start();
        }

        public delegate void StatisticsUpdatedDelegate();
        public event StatisticsUpdatedDelegate StatisticsUpdated;

        bool Work = true;
        public bool Suspend = false;
        public void Update()
        {
            while(Work)
            {
                if (!Suspend)
                {
                    Save((Environment.TickCount - StartTime) / 1000);
                    if (StatisticsUpdated != null)
                        StatisticsUpdated();
                }
                System.Threading.Thread.Sleep(500);
            }
        }

        public void Save(double Time)
        {
            if (MainForm.Host != null && Ready)
            {
                Data["Connections"].Add(new Dataset() { Time = Time, Value = MainForm.Host.netServer.ConnectionsCount });

                Data["ReceivedBytes"].Add(new Dataset() { Time = Time, Value = MainForm.Host.netServer.Statistics.ReceivedBytes });
                Data["ReceivedMessages"].Add(new Dataset() { Time = Time, Value = MainForm.Host.netServer.Statistics.ReceivedMessages });
                Data["ReceivedPackets"].Add(new Dataset() { Time = Time, Value = MainForm.Host.netServer.Statistics.ReceivedPackets });

                Data["SentBytes"].Add(new Dataset() { Time = Time, Value = MainForm.Host.netServer.Statistics.SentBytes });
                Data["SentMessages"].Add(new Dataset() { Time = Time, Value = MainForm.Host.netServer.Statistics.SentMessages });
                Data["SentPackets"].Add(new Dataset() { Time = Time, Value = MainForm.Host.netServer.Statistics.SentPackets });

                Data["StorageBytesAllocated"].Add(new Dataset() { Time = Time, Value = MainForm.Host.netServer.Statistics.StorageBytesAllocated });
                Data["BytesInRecyclePool"].Add(new Dataset() { Time = Time, Value = MainForm.Host.netServer.Statistics.BytesInRecyclePool });

                Data["GameObjectCount"].Add(new Dataset() { Time = Time, Value = MainForm.Host.World.AllGameObjects.Count });
                Data["ConnectedClients"].Add(new Dataset() { Time = Time, Value = MainForm.Host.ConnectedClients.Count });
            }

            if (MainForm._mainGame != null)
            {
                Data["Tickrate"].Add(new Dataset() { Time = Time, Value = MainForm._mainGame.Tickrate });
            }

            foreach(var key in Data.Keys)
            {
                for (int i = 0; i < Data[key].Count; i++ )
                {
                    if (Data[key][i].Time < DumpTime)
                    {
                        Data[key].RemoveAt(i);
                    }
                }
            }

            if (MainForm.Host != null)
                MainForm.Host.netServer.Statistics.Reset();
        }
        public struct Dataset
        {
            public double Time;
            public float Value;
        }
    }
}
