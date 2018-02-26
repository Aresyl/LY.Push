using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LY.Push.PushPlugin.PushSharpIosService.Models
{
    [DataContract]
    [Serializable]
    public class iOSPushSendCallbackModel
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public bool SendResult { get; set; }
    }
}
