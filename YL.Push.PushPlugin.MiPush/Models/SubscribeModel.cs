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
    public class SubscribeBaseModel
    {
        [DataMember]
        public string category { get; set; }
        [DataMember]
        public string topic { get; set; }
        [DataMember]
        public string restricted_package_name { get; set; }
        
    }
    [DataContract]
    [Serializable]
    public class SubscribeRegidModel: SubscribeBaseModel
    {
        
        [DataMember]
        public string registration_id { get; set; }
        
    }
    [DataContract]
    [Serializable]
    public class SubscribeAliasModel : SubscribeBaseModel
    {
       
        [DataMember]
        public string aliases { get; set; }
    }
}
