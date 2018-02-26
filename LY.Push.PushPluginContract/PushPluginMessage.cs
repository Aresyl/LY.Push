namespace LY.Push.PushPluginContract
{
    /// <summary>
    /// 推送消息
    /// </summary>
    public class PushPluginMessage
    {
        /// <summary>
        /// 设备token值
        /// </summary>
        public string Token
        {
            get; set;
        }

        /// <summary>
        /// 在通知栏展示的通知的标题
        /// </summary>
        public string Title
        {
            get; set;
        }

        /// <summary>
        /// 推送内容
        /// </summary>
        public string Message
        {
            get; set;
        }

        /// <summary>
        /// 在通知栏展示的通知的描述
        /// </summary>
        public string Description
        {
            get; set;
        }

        /// <summary>
        /// 设置消息是否通过透传的方式送给app，1表示透传消息，0表示通知栏消息
        /// </summary>
        public int PassThrough
        {
            get; set;
        }

        /// <summary>
        /// 客户端接收到消息，点击后执行的动作
        /// </summary>
        public int ClientAction
        {
            get; set;
        }
        public string ClientActionUrl
        {
            get; set;
        }
        /// <summary>
        /// 推送批次标识
        /// </summary>
        public string BatchNo
        {
            get; set;
        }


    }
}