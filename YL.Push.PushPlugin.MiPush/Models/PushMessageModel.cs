using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YL.Push.PushPlugin.MiPush.Models
{
    public class PushMessageModel
    {
        public int id
        {
            get; set;
        }
        /// <summary>
        /// 在通知栏展示的通知的标题
        /// </summary>
        public string title
        {
            get; set;
        }

        /// <summary>
        /// 推送内容
        /// </summary>
        public string content
        {
            get; set;
        }

        /// <summary>
        /// 在通知栏展示的通知的描述
        /// </summary>
        public string url
        {
            get; set;
        }

        /// <summary>
        /// 设置消息是否通过透传的方式送给app，1表示透传消息，0表示通知栏消息
        /// </summary>
        public int action
        {
            get; set;
        }
    }
}
