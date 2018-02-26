using System;
using System.Runtime.Serialization;

namespace LY.Push.PushPluginContract
{
    /// <summary>
    /// Token模板
    /// </summary>
    [DataContract]
    [Serializable]
    public class TokenModel
    {
        [DataMember]
        public string Token { get; set; }
        /// <summary>
        /// 推送批次标识
        /// </summary>
        [DataMember]
        public string BatchNo { get; set; }
    }
}