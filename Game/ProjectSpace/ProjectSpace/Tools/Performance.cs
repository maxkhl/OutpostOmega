using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace OutpostOmega.Tools
{
    static class Performance
    {
        public class Counter
        {
            Stopwatch Watch;

            /// <summary>
            /// Gets or sets the round trip time. (in ms)
            /// </summary>
            public long RoundTripTime { get; set; }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            public string Name { get; set; }

            public Counter(string Name)
            {
                this.Name = Name;
                Watch = new Stopwatch();
            }

            public void Start()
            {
                Watch.Start();
            }

            public void Stop()
            {
                RoundTripTime = Watch.ElapsedMilliseconds;
                Watch.Reset();
            }
        }

        public static List<Counter> CounterList = new List<Counter>();

        public static Counter GetNewCounter(string Name)
        {
            var retCounter = new Counter(Name);
            CounterList.Add(retCounter);
            return retCounter;
        }

        public static void Start(string Name)
        {
            var tCounter = (from counter in CounterList
                            where counter.Name == Name
                            select counter).SingleOrDefault();
            if (tCounter == null)
                tCounter = GetNewCounter(Name);

            tCounter.Start();
        }

        public static void Stop(string Name)
        {
            var tCounter = (from counter in CounterList
                            where counter.Name == Name
                            select counter).SingleOrDefault();
            if (tCounter != null)
                tCounter.Stop();
        }
    }
}
