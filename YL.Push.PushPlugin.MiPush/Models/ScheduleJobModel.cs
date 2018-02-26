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
    public class ScheduleJobModel
    {
        [DataMember]
        public string job_id { get; set; }
    }
}
