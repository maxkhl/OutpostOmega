using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.CompilerServices;

namespace OutpostOmega.Data
{
    /// <summary>
    /// This part handles the object identification. It is used to determine object relations and serialize referring properties correctly
    /// </summary>
    public abstract partial class cConverter
    {
        /// <summary>
        /// Used to get a unique identifier and recognize objects, that are already serialized
        /// </summary>
        //private ObjectIDGenerator objectIDGenerator = new ObjectIDGenerator();

        /// <summary>
        /// Used to get a unique identifier and recognize objects, that are already serialized
        /// </summary>
        private Dictionary<string, Dictionary<string, object>> _objectData = new Dictionary<string, Dictionary<string, object>>();

        /// <summary>
        /// Contains yet unidentified references for this converter
        /// This has to be checked on every deserialization of an object to resolve this issue
        /// At the end of the deserialization progress this should be empty (if not, the save file is messed up probably)
        /// </summary>
        public List<UnidentifiedReference> UnidentifiedReferences = new List<UnidentifiedReference>();

        /// <summary>
        /// Enqueues a reference for later processing
        /// </summary>
        public void EnqueueReference(string Sender, string Identity, System.Reflection.PropertyInfo TargetProperty, object TargetInstance, System.Xml.Linq.XElement Data)
        {
            UnidentifiedReferences.Add(new UnidentifiedReference()
                {
                    Sender = Sender,
                    Identity = Identity,
                    TargetProperty = TargetProperty,
                    TargetInstance = TargetInstance,
                    Data = Data,
                });
        }

        public void TryResolveReferences(string Identity, object SourceObject)
        {
            var matchingReferences = (from reference in UnidentifiedReferences
                                      where
                                        reference.Identity == Identity && // Same identity?
                                        reference.TargetProperty.PropertyType == SourceObject.GetType() // Same type?
                                      select reference);

            foreach(var matchingReference in matchingReferences)
            {
                matchingReference.TargetProperty.SetValue(matchingReference.TargetInstance, SourceObject);

                //Reference resolved
                UnidentifiedReferences.Remove(matchingReference);
            }
        }

        /// <summary>
        /// Checks a specific object for its unique identifier
        /// </summary>
        /// <param name="senderID">ID of the sender</param>
        /// <param name="obj">The object that should be checked</param>
        /// <param name="firstTime">Returns true, if the object is already known</param>
        /// <returns>The unique identifier of this object</returns>
        protected string CheckObject(string senderID, object obj, out bool firstTime)
        {
            firstTime = true;

            if (_objectData == null)
                throw new Exception("Converter seems to be already disposed");

            if (!_objectData.ContainsKey(senderID))
                _objectData.Add(senderID, new Dictionary<string, object>());

            string ID = "";
            ID = GetID(obj);
            if (_objectData[senderID].ContainsKey(ID))
            {
                firstTime = false;
            }
            else
            {
                firstTime = true;
                _objectData[senderID].Add(ID, obj);
            }

            return ID;
        }

        /// <summary>
        /// Used to determine a unique identification-string for this object. Default method will search for an ID
        /// </summary>
        /// <param name="obj">the object</param>
        /// <returns>the unique id of the given object</returns>
        virtual public string GetID(object obj)
        {

            var typeofObject = obj.GetType();
            
            string ID = "";
            if (typeofObject.IsSerializable)
            {
                ID = RuntimeHelpers.GetHashCode(obj).ToString();
            }
            else
            {
                var properties = typeofObject.GetProperties();

                var oID = (from vID in properties
                           where vID.Name == "ID"
                           select vID.GetValue(obj, null)).FirstOrDefault();

                if (oID != null && typeof(string) == oID.GetType())
                    ID = (string)oID;
                else
                    ID = RuntimeHelpers.GetHashCode(obj).ToString();
            }
            if(ID == "")
                throw new Exception("Default GetID()-Method not working here. Please override the method with your own function.");
            return ID;
        }

        /// <summary>
        /// Checks if a object is known by the IDGenerator without introducing it. Returns the ID if it is known
        /// </summary>
        /// <param name="senderID">ID of the sender</param>
        /// <param name="obj">object</param>
        /// <param name="firstTime">Was this the first check?</param>
        /// <returns>Instance ID</returns>
        protected string CheckObjectPeek(string senderID, object obj, out bool firstTime)
        {
            if (_objectData == null)
                throw new Exception("Converter seems to be already disposed");
            
            if (!_objectData.ContainsKey(senderID))
                _objectData.Add(senderID, new Dictionary<string, object>());

            string ID = "";
            ID = GetID(obj);
            if (_objectData[senderID].ContainsKey(ID))
            {
                firstTime = false;
            }
            else
            {
                firstTime = true;
            }

            return ID;
        }

        /// <summary>
        /// Checks if a specific instance ID exists in the buffer
        /// </summary>
        /// <param name="senderID">ID of the sender</param>
        /// <param name="instanceID">Instance ID</param>
        /// <returns>true = found</returns>
        protected bool HasInstance(string senderID, string instanceID)
        {
            lock (_objectData)
                if (!_objectData.ContainsKey(senderID))
                    _objectData.Add(senderID, new Dictionary<string, object>());

            return _objectData[senderID].ContainsKey(instanceID);
        }

        /// <summary>
        /// Gets the instance assigned to the given ID
        /// </summary>
        /// <param name="senderID">ID of the sender</param>
        /// <param name="instanceID">ID of target instance</param>
        /// <returns>found object. Null if not found</returns>
        protected object GetInstance(string senderID, string instanceID)
        {
            if (!_objectData.ContainsKey(senderID))
                _objectData.Add(senderID, new Dictionary<string, object>());

            if (_objectData[senderID].ContainsKey(instanceID))
                return _objectData[senderID][instanceID];
            else
                return null;
        }

        /// <summary>
        /// Introduces an Object to the Object Buffer and the IDGenerator
        /// </summary>
        /// <param name="senderID">ID of the sender</param>
        /// <param name="obj">Object</param>
        protected void RegisterObject(string senderID, object obj, string ID = "")
        {
            if (!_objectData.ContainsKey(senderID))
                _objectData.Add(senderID, new Dictionary<string, object>());

            if (ID == "")
                ID = GetID(obj);

            if (!_objectData[senderID].ContainsKey(ID))
                _objectData[senderID].Add(ID, obj);
        }

        /// <summary>
        /// Gets the ID of a specific object
        /// </summary>
        /// <param name="obj">Object</param>
        /// <returns>ID of the object. "" if not possible</returns>
        public static string GetDefID(object obj)
        {
            LoadConverter();

            // Get converter
            cConverter targetConverter = (from conv in AllConverter
                                         where conv.CheckCompatibility(obj)
                                         select conv).FirstOrDefault();

            if(targetConverter != null)
            {
                targetConverter.GetID(obj);
            }

            return "";
        }

        /// <summary>
        /// Contains a reference that is not yet serializable
        /// </summary>
        public struct UnidentifiedReference
        {
            public string Sender { get; set; }
            public string Identity { get; set; }
            public System.Reflection.PropertyInfo TargetProperty { get; set; }
            public object TargetInstance { get; set; }
            public System.Xml.Linq.XElement Data { get; set; }
        }
    }
}
