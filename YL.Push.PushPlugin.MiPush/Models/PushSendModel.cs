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
    public class PushSendBaseModel
    {
        [DataMember]
        public string payload { get; set; }
        [DataMember]
        public string restricted_package_name { get; set; }
        [DataMember]
        public int pass_through { get; set; }

        [DataMember]
        public string title { get; set; }

        [DataMember]
        public string description { get; set; }
        [DataMember]
        public int notify_type { get; set; }
    }
    [DataContract]
    [Serializable]
    public class PushSendAliasModel : PushSendBaseModel
    {
        [DataMember]
        public string alias { get; set; }
    }
    [DataContract]
    [Serializable]
    public class PushSendRegidModel : PushSendBaseModel
    {
        [DataMember]
        public string registration_id { get; set; }
    }

    [DataContract]
    [Serializable]
    public class PushResultModel
    {
        [DataMember]
        public string result { get; set; }
        [DataMember]
        public PushResultDataModel data { get; set; }

    }
    [DataContract]
    [Serializable]
    public class PushResultDataModel
    {
        [DataMember]
        public string id { get; set; }
    }
}
