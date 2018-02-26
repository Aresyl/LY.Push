using System;
using System.Collections.Generic;

namespace LY.Push.PushPluginContract
{
    public interface IPushPlugin
    {
        event EventHandler<PushCompletedArgs> PushCompleted;

        /// <summary>
        /// 初始化推送服务
        /// </summary>
        /// <param name="host">服务提供地址</param>
        /// <param name="appId">App注册的推送服务id</param>
        /// <param name="appKey">推送服务密码</param>
        /// <param name="appCertpath">推送服务使用密钥</param>
        /// <returns>是否初始化成功</returns>
        bool InitialPushProvider(string host, string appId, string appKey, string appCertpath);

        /// <summary>
        /// 给不同的用户推送同一条消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="tokens"></param>
        /// <returns></returns>
        bool Send(PushPluginMessage message, IEnumerable<TokenModel> tokens);

        /// <summary>
        /// 给不同的用户推送不同的消息
        /// </summary>
        /// <param name="messages"></param>
        /// <returns></returns>
        bool Send(IEnumerable<PushPluginMessage> messages);

        /// <summary>
        /// 设置别名
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="tokens"></param>
        /// <returns></returns>
        bool SetAlia(string alias, IEnumerable<string> tokens);

        /// <summary>
        /// 删除别名
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        bool DeleteAlia(string alias);

        /// <summary>
        /// 设置标签
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="tokens"></param>
        /// <returns></returns>
        bool SetTag(string tagName, IEnumerable<string> tokens);

        /// <summary>
        /// 删除标签
        /// </summary>
        /// <param name="tagName"></param>
        /// <returns></returns>
        bool DeleteTag(string tagName);

    }
}