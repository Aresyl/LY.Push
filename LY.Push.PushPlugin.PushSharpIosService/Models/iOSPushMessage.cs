using System;
using System.Runtime.Serialization;

namespace LY.Push.PushPlugin.PushSharpIosService.Models
{
    /// <summary>
    /// iOS推送消息
    /// </summary>
    [DataContract]
    [Serializable]
    public class iOSPushMessage
    {
        /// <summary>
        /// 设备token值
        /// </summary>
        [DataMember]
        public string Token { get; set; }

        /// <summary>
        /// 推送消息内容(针对IOS有效)
        /// </summary>
        [DataMember]
        public string Body { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string BatchNo { get; set; }

        /// <summary>
        /// 是否使用回调，IOS专用
        /// </summary>
        [DataMember]
        public bool IsUserToken { get; set; }
    }
}
