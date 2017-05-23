using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutpostOmega.Game.datums
{
    /// <summary>
    /// A UserGroup, a mind can be assigned to
    /// </summary>
    public enum UserGroup
    {
        Administrator = 9,
        Gamemaster = 5,
        Moderator = 4,
        VIP = 2,
        User = 1,
        Observer = 0
    }
    
    /// <summary>
    /// Static helper functions
    /// </summary>
    public static class UserGroupTools
    {
        /// <summary>
        /// Checks if the given 'Group' has access to the 'Target'-usergroup
        /// </summary>
        public static bool HasAccess(UserGroup Target, UserGroup Group)
        {
            return (int)Group >= (int)Target;
        }

        /// <summary>
        /// Determins the type of a accessviolation
        /// </summary>
        public enum AccessViolationType
        {
            Critical,
            Normal,
            Minor,
        }
        
        /// <summary>
        /// Used to store a single unauthorized access
        /// </summary>
        public struct AccessViolation
        {
            public AccessViolationType Type;
            public string Message;
            public DateTime Time;
            public object Data;
        }

        /// <summary>
        /// Contains all access violations that occured while this program was running
        /// </summary>
        public static Queue<AccessViolation> AccessViolations = new Queue<AccessViolation>();

        public delegate void NewAccessViolationHandler(AccessViolation accessViolation);

        /// <summary>
        /// Used to capture and react to a access violation right when it happens
        /// </summary>
        public static event NewAccessViolationHandler NewAccessViolation;

        /// <summary>
        /// Logs a access violation
        /// </summary>
        public static void LogAccessViolation(AccessViolationType Type, string Message, object Data)
        {
            var newAV = new AccessViolation()
                {
                    Type = Type,
                    Message = Message,
                    Data = Data,
                    Time = DateTime.Now,
                };

            AccessViolations.Enqueue(newAV);

            if (NewAccessViolation != null)
                NewAccessViolation(newAV);
        }
    }
}
