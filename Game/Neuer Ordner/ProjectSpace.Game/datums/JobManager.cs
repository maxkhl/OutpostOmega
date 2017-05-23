using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutpostOmega.Game.datums
{
    public class JobManager : datum
    {
        public enum Departements
        {
            Civil,
            Medical,
            Science,
            Engineering,
            Security,
            Command
        }

        public struct Job
        {
            public string Title;
            public string Description;
            public string Supervisor;
            public int TotalPositions;
            public Departements Departement;
            public int[] Access;
        }

        public JobManager(World World)
            : base(World)
        { }

        public List<Job> JobList = GetJobs();

        private static List<Job> GetJobs()
        {
            var JobList = new List<Job>();
            JobList.Add(new Job
                {
                    Title = "Assistant",
                    Description = "Basic access. A normal assistant that should try to assist others.",
                    Supervisor = "everyone",
                    TotalPositions = -1, //Unlimited
                    Departement = Departements.Civil,
                    Access = new int[2]
                    {
                        1, 2
                    }
                });

            JobList.Add(new Job
            {
                Title = "Captain",
                Description = "Master of disaster",
                Supervisor = "Administration, Space Law",
                TotalPositions = 1,
                Departement = Departements.Command,
                Access = new int[2]
                    {
                        1, 2
                    }
            });

            return JobList;
        }
    }
}
