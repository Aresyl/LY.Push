using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LY.Push.PushPlugin.PushSharpIosService.Models;
using LY.Push.PushPluginContract;

namespace LY.Push.PushPlugin.PushSharpIosService
{
    public class PluginMain : IiOSPushPlugin
    {
        public event EventHandler<PushCompletedArgs> PushCompleted;
        /// <summary>
        /// 是否测试环境:true(测试环境)、false(正式环境)
        /// </summary>
        private readonly bool _isTest = string.IsNullOrEmpty(ConfigurationManager.AppSettings["IsTest"]) || ConfigurationManager.AppSettings["IsTest"] == "true";
        /// <summary>
        /// Apple消息推送Payload
        /// </summary>
        private readonly string _alertSchema = "{\"aps\":{\"alert\":\"{0}\"}}";
        private readonly string _pushSchema = "{\"id\":0,\"type\":{1},\"url\":\"{2}\",\"title\":\"{3}\"}";

        private PushSharpIosService _pushService;

        /// <summary>
        /// 证书密码
        /// </summary>
        private static string _appleCertPwd = "";

        /// <summary>
        /// 初始化推送服务
        /// </summary>
        /// <param name="host">服务提供地址</param>
        /// <param name="appId">App注册的推送服务id</param>
        /// <param name="appKey">推送服务密码</param>
        /// <param name="appCertpath">推送服务使用密钥</param>
        /// <returns>是否初始化成功</returns>
        public bool InitialPushProvider(string host, string appId, string appKey, string appCertpath)
        {
            var ret = false;
            if (!string.IsNullOrEmpty(appCertpath))
            {
                _appleCertPwd = appCertpath;

                if (_pushService == null)
                {
                    _pushService = new PushSharpIosService(_appleCertPwd, _isTest);
                    _pushService.PushSucceeded += PushService_PushSucceeded;
                    _pushService.PushFailed += PushService_PushFailed;
                }

                ret = true;
            }

            return ret;
        }
        #region 一定要实现
        /// <summary>
        /// 给不同的用户推送同一条消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="tokens"></param>
        /// <returns></returns>
        public bool Send(PushPluginMessage message, IEnumerable<TokenModel> tokens)
        {
            //将ios推送的消息值添加到ios推送列表中
            var listMessage = new List<iOSPushMessage>();
            var ret = false;
            foreach (var token in tokens)
            {
                // 推送消息内容
                listMessage.Add(new iOSPushMessage
                {
                    Token = token.Token,
                    Message = "{\"id\":0,\"type\":" + message.ClientAction + ",\"url\":\"" + message.ClientActionUrl + "\",\"title\":\"" + message.Title + "\"}",
                    Body = message.Message,
                    BatchNo = token.BatchNo,
                });
            }


            //if(_pushService == null)
            //{
            //	_pushService = new PushSharpIosService(_appleCertPwd, _isTest);
            //	_pushService.PushSucceeded += PushServce_PushSucceeded;
            //	_pushService.PushFailed += PushServce_PushFailed;
            //}

            // 发起ios推送请求(PushSharp方式)
            _pushService.SendMessage(listMessage);
            ret = true;
            return ret;
        }

        /// <summary>
        /// 给不同的用户推送不同的消息
        /// </summary>
        /// <param name="messages"></param>
        /// <returns></returns>
        public bool Send(IEnumerable<PushPluginMessage> messages)
        {
            //将ios推送的消息值添加到ios推送列表中
            var listMessage = new List<iOSPushMessage>();
            var ret = false;
            foreach (var m in messages)
            {
                // 推送消息内容
                listMessage.Add(new iOSPushMessage
                {
                    Token = m.Token,
                    Message = "{\"id\":0,\"type\":" + m.ClientAction + ",\"title\":\"" + m.Title + "\",\"url\":\"" + m.ClientActionUrl + "\"}",
                    Body = m.Message,
                    BatchNo = m.BatchNo,
                });
            }

            //if(_pushService == null)
            //{
            //	_pushService = new PushSharpIosService(_appleCertPwd, _isTest);
            //	_pushService.PushSucceeded += PushServce_PushSucceeded;
            //	_pushService.PushFailed += PushServce_PushFailed;
            //}

            // 发起ios推送请求(PushSharp方式)
            _pushService.SendMessage(listMessage);
            ret = true;
            return ret;
        }
        #endregion
        /// <summary>
        /// 设置别名
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="tokens"></param>
        /// <returns></returns>
        public bool SetAlia(string alias, IEnumerable<string> tokens)
        {
            var ret = false;

            return ret;
        }

        /// <summary>
        /// 删除别名
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        public bool DeleteAlia(string alias)
        {
            var ret = false;

            return ret;
        }
        /// <summary>
        /// 设置标签
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="tokens"></param>
        /// <returns></returns>
        public bool SetTag(string tagName, IEnumerable<string> tokens)
        {
            var ret = false;

            return ret;
        }

        /// <summary>
        /// 删除标签
        /// </summary>
        /// <param name="tagName"></param>
        /// <returns></returns>
        public bool DeleteTag(string tagName)
        {
            var ret = false;

            return ret;
        }

        #region 发送结果
        private void PushService_PushSucceeded(object sender, string e)
        {
            PushSucess(e);
        }
        private void PushService_PushFailed(object sender, string e)
        {
            PushFail(e);
        }

        public void PushSucess(string batchNo)
        {
            var handler = PushCompleted;

            if (handler != null)
            {
                var args = new PushCompletedArgs()
                {
                    BatchNo = batchNo,
                    IsSuccess = true,
                };
                handler(this, args);
            }

        }
        public void PushFail(string batchNo)
        {
            var handler = PushCompleted;

            if (handler != null)
            {
                var args = new PushCompletedArgs()
                {
                    BatchNo = batchNo,
                    IsSuccess = false,
                };
                handler(this, args);
            }
        }
        #endregion
    }
}
