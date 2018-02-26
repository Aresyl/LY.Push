using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace YL.Push.PushPlugin.MiPush.Models
{
    /// <summary>
    /// 返回结果
    /// </summary>
    [DataContract]
    [Serializable]
    public class ResultModel
    {
        /// <summary>
        /// 操作结果
        /// </summary>
        [DataMember]
        public bool Success { get; set; }

        /// <summary>
        /// 错误状态码
        /// </summary>
        [DataMember]
        public string ErrorCode { get; set; }

        /// <summary>
        /// 错误信息，成功将返回空
        /// </summary>
        [DataMember]
        public string ErrorMessage { get; set; }
        string _serviceTime = DateTime.Now.ToString("s");
        /// <summary>
        /// 统一返回当前服务器时
        /// </summary>
        [DataMember]
        public string ServiceTime
        {
            get { return _serviceTime; }
            set
            {
                if (value == null)
                {
                    value = DateTime.Now.ToString("s");
                }
                _serviceTime = value;
            }
        }

    }

    /// <summary>
    /// 返回结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DataContract]
    [Serializable]
    public class ResultModel<T> : ResultModel
    {
        /// <summary>
        /// 数据
        /// </summary>
        [DataMember]
        public T Data { get; set; }
    }
}
