using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace LY.Push.PushPlugin.PushSharpIosService.Models
{
    [DataContract]
    [Serializable]
    public class SendIosModel
    {
        [DataMember]
        public ApsModel aps { get; set; }
        [DataMember]
        public ActionModel action { get; set; }
    }
    [DataContract]
    [Serializable]
    public class ApsModel
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string alert { get; set; }

        /// <summary>
        /// App Logo上面显示的消息数量
        /// </summary>
        [DataMember]
        public int badge { get; set; }

        /// <summary>
        /// 音频文件
        /// </summary>
        [DataMember]
        public string sound { get; set; }
    }
    [DataContract]
    [Serializable]
    public class ActionModel
    {
        [DataMember]
        public int id { get; set; }
        [DataMember]
        public int type { get; set; }
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string url { get; set; }
    }
}
