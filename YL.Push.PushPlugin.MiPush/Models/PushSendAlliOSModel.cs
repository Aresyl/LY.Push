using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace YL.Push.PushPlugin.MiPush.Models
{
    [DataContract]
    [Serializable]
    public class PushSendAlliOSModel
    {
        [DataMember]
        public string AppSecret { get; set; }
        [DataMember]
        public string payload { get; set; }
        [DataMember]
        public string description { get; set; }
        [DataMember]
        public string restricted_package_name { get; set; }

        

    }
}
