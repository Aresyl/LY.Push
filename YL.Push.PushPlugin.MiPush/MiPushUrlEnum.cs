using System;
using System.ComponentModel;
using System.Reflection;

namespace YL.Push.PushPlugin.MiPush
{
    public enum MiPushUrlEnum : byte
    {
        #region 正式环境V2版接口：
        [Description("向某个regid或一组regid列表推送某条消息")]
        [MiPushUrl("v2/message/regid")]
        RegidV2 = 1,

        [Description("向某个alias或一组alias列表推送某条消息")]
        [MiPushUrl("v2/message/alias")]
        AliasV2 = 2,

        [Description("向某个account或一组account列表推送某条消息")]
        [MiPushUrl("v2/message/user_account")]
        UserAccountV2 = 3,

        [Description("向多个topic推送单条消息")]
        [MiPushUrl("v2/message/multi_topic")]
        TopicV2 = 4,

        [Description("向所有设备推送某条消息")]
        [MiPushUrl("v2/message/all")]
        AllV2 = 5,
        #endregion

        #region V3版接口（兼容V2版接口并支持多包名)
        [Description("向某个regid或一组regid列表推送某条消息（这些regId可以属于不同的包名）")]
        [MiPushUrl("v3/message/regid")]
        RegidV3 = 6,

        [Description("向某个alias或一组alias列表推送某条消息（这些alias可以属于不同的包名）")]
        [MiPushUrl("v3/message/alias")]
        AliasV3 = 7,

        [Description("向某个topic推送某条消息（可以指定一个或多个包名）")]
        [MiPushUrl("v3/message/topic")]
        TopicV3 = 8,

        [Description("向多个topic推送单条消息（可以指定一个或多个包名）")]
        [MiPushUrl("v3/message/multi_topic")]
        MultiTopicV3 = 9,

        [Description("向所有设备推送某条消息（可以指定一个或多个包名）")]
        [MiPushUrl("v3/message/all")]
        AllV3 = 10,
        #endregion

        [Description("GET 获取消息的统计数据正式环境API地址：")]
        [MiPushUrl("v1/stats/message/counters")]
        Counters = 11,
        [Description("GET 追踪消息")]
        [MiPushUrl("v1/trace/message/status")]
        MessageStatus = 12,
        [Description("GET 获取消息的统计数据正式环境API地址：")]
        [MiPushUrl("v1/trace/messages/status")]
        MessagesStatus = 13,

        #region 订阅/取消订阅标签
        [Description("正式环境订阅RegId的标签")]
        [MiPushUrl("v2/topic/subscribe")]
        SubscribeRegId = 14,

        [Description("取消订阅RegId的标签")]
        [MiPushUrl("v2/topic/unsubscribe")]
        UnsubscribeRegId = 15,

        [Description("正式环境订阅RegId的标签")]
        [MiPushUrl("v2/topic/subscribe/alias")]
        SubscribeAlias = 16,

        [Description("取消订阅RegId的标签")]
        [MiPushUrl("v2/topic/unsubscribe/alias")]
        UnsubscribeAlias = 17,

        [Description("检测定时任务是否存在")]
        [MiPushUrl("v2/schedule_job/exist")]
        ScheduleJobExist = 18,

        [Description("删除定时任务")]
        [MiPushUrl("v2/schedule_job/delete")]
        ScheduleJobDelete = 19,
        #endregion

    }
    public class MiPushUrlAttribute : Attribute
    {
        private string _miPushUrl;

        public MiPushUrlAttribute(string htmlFileName)
        {
            this._miPushUrl = htmlFileName;
        }

        public string HtmlFileName
        {
            get { return _miPushUrl; }
            set { _miPushUrl = value; }
        }
    }

    public static class EnumExtension
    {
        /// <summary>
        /// 获取枚举的扩展属性：协议html文件名称
        /// </summary>
        /// <param name="em"></param>
        /// <returns></returns>
        public static string GetPushUrl(this System.Enum em)
        {
            Type temType = em.GetType();
            MemberInfo[] memberInfos = temType.GetMember(em.ToString());
            if (memberInfos.Length > 0)
            {
                object[] objs = memberInfos[0].GetCustomAttributes(typeof(MiPushUrlAttribute), false);
                if (objs.Length > 0)
                {
                    return ((MiPushUrlAttribute)objs[0]).HtmlFileName;
                }
            }
            return em.ToString();
        }
    }
}