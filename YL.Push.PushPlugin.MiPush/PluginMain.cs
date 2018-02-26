using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using LY.Push.PushPluginContract;
using Newtonsoft.Json;
using YL.Push.PushPlugin.MiPush.Models;

namespace YL.Push.PushPlugin.MiPush
{
    public class PluginMain : IAndroidPushPlugin
    {
        private ILog _log = LogManager.GetLogger(typeof(PluginMain));
        public event EventHandler<PushCompletedArgs> PushCompleted;
        MiPushBll _miPushBll = new MiPushBll();
        private static readonly string MiPushBaseUrl = @"https://api.xmpush.xiaomi.com/";
        private static string _appSecret = string.Empty;
        private static readonly string RestrictedPackageName = "com.yl.app";

        #region 对接

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
                _appSecret = appCertpath;
                ret = true;
            }
            return ret;
        }

        /// <summary>
        /// 给不同的用户推送同一条消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="tokens"></param>
        /// <returns></returns>
        public bool Send(PushPluginMessage message, IEnumerable<TokenModel> tokens)
        {
            var thread = new Thread(() => SendUniformMessage(message, tokens));
            thread.Start();

            return true;
        }

        /// <summary>
        /// 给不同的用户推送不同的消息
        /// </summary>
        /// <param name="messages"></param>
        /// <returns></returns>
        public bool Send(IEnumerable<PushPluginMessage> messages)
        {
            var thread = new Thread(() => SendDiffMessage(messages));
            thread.Start();

            return true;
        }

        /// <summary>
        /// 设置别名
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="tokens"></param>
        /// <returns></returns>
        public bool SetAlia(string alias, IEnumerable<string> tokens)
        {
            var ret = false;
            var sendData = new SubscribeAliasModel()
            {
                aliases = string.Join(",", tokens),
                topic = alias,
                restricted_package_name = RestrictedPackageName,
            };
            var sentResult = Subscribe(sendData);
            if (sentResult.result.Equals("ok"))
            {
                ret = true;
            }
            return ret;
        }

        /// <summary>
        /// 删除别名
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        public ResultModel<string> DeleteAlia(string alias, IEnumerable<string> tokens)
        {
            var ret = new ResultModel<string>();
            var sendData = new SubscribeAliasModel()
            {
                aliases = string.Join(",", tokens),
                topic = alias,
                restricted_package_name = RestrictedPackageName,
            };
            var sentResult = Unsubscribe(sendData);
            if (sentResult.result.Equals("ok"))
            {
                ret.Success = true;
                ret.Data = sentResult.data.id;
            }
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
            var sendData = new SubscribeRegidModel()
            {
                registration_id = string.Join(",", tokens),
                topic = tagName,
                restricted_package_name = RestrictedPackageName,
            };
            var sentResult = Subscribe(sendData);
            if (sentResult.result.Equals("ok"))
            {
                ret = true;
            }
            return ret;
        }

        /// <summary>
        /// 删除标签
        /// </summary>
        /// <param name="tagName"></param>
        /// <returns></returns>
        public ResultModel<string> DeleteTag(string tagName, IEnumerable<string> tokens)
        {
            var ret = new ResultModel<string>();
            var sendData = new SubscribeRegidModel()
            {
                registration_id = string.Join(",", tokens),
                topic = tagName,
                restricted_package_name = RestrictedPackageName,
            };
            var sentResult = Unsubscribe(sendData);
            if (sentResult.result.Equals("ok"))
            {
                ret.Success = true;
                ret.Data = sentResult.data.id;
            }
            return ret;
        }

        #endregion

        public PushResultModel Subscribe(SubscribeAliasModel messages)
        {
            var ret = new PushResultModel();
            if (messages == null || string.IsNullOrEmpty(_appSecret))
                return ret;

            const MiPushUrlEnum miPushUrlType = (MiPushUrlEnum)16;
            var urlChile = miPushUrlType.GetPushUrl();
            var url = Path.Combine(MiPushBaseUrl, urlChile);

            var postParams = GetProperties<SubscribeAliasModel>(messages);
            var retString = _miPushBll.Send(url, postParams, _appSecret);
            if (!string.IsNullOrEmpty(retString))
            {
                ret = JsonConvert.DeserializeObject<PushResultModel>(retString);
            }
            return ret;
        }

        public PushResultModel Unsubscribe(SubscribeAliasModel messages)
        {
            var ret = new PushResultModel();
            if (messages == null || string.IsNullOrEmpty(_appSecret))
                return ret;

            const MiPushUrlEnum miPushUrlType = (MiPushUrlEnum)17;
            var urlChile = miPushUrlType.GetPushUrl();
            var url = Path.Combine(MiPushBaseUrl, urlChile);
            var postParams = GetProperties<SubscribeAliasModel>(messages);
            var retString = _miPushBll.Send(url, postParams, _appSecret);
            if (!string.IsNullOrEmpty(retString))
            {
                ret = JsonConvert.DeserializeObject<PushResultModel>(retString);
            }
            return ret;
        }

        public PushResultModel Subscribe(SubscribeRegidModel messages)
        {
            var ret = new PushResultModel();
            if (messages == null || string.IsNullOrEmpty(_appSecret))
                return ret;

            const MiPushUrlEnum miPushUrlType = (MiPushUrlEnum)14;
            var urlChile = miPushUrlType.GetPushUrl();
            var url = Path.Combine(MiPushBaseUrl, urlChile);

            var postParams = GetProperties<SubscribeRegidModel>(messages);
            var retString = _miPushBll.Send(url, postParams, _appSecret);
            if (!string.IsNullOrEmpty(retString))
            {
                ret = JsonConvert.DeserializeObject<PushResultModel>(retString);
            }
            return ret;
        }

        public PushResultModel Unsubscribe(SubscribeRegidModel messages)
        {
            var ret = new PushResultModel();
            if (messages == null || string.IsNullOrEmpty(_appSecret))
                return ret;

            const MiPushUrlEnum miPushUrlType = (MiPushUrlEnum)15;
            var urlChile = miPushUrlType.GetPushUrl();
            var url = Path.Combine(MiPushBaseUrl, urlChile);
            var postParams = GetProperties<SubscribeRegidModel>(messages);
            var retString = _miPushBll.Send(url, postParams, _appSecret);
            if (!string.IsNullOrEmpty(retString))
            {
                ret = JsonConvert.DeserializeObject<PushResultModel>(retString);
            }
            return ret;
        }

        public PushResultModel ScheduleJobExist(ScheduleJobModel messages)
        {
            var ret = new PushResultModel();
            if (messages == null || string.IsNullOrEmpty(_appSecret))
                return ret;

            const MiPushUrlEnum miPushUrlType = (MiPushUrlEnum)18;
            var urlChile = miPushUrlType.GetPushUrl();
            var url = Path.Combine(MiPushBaseUrl, urlChile);
            var postParams = GetProperties<ScheduleJobModel>(messages);
            var retString = _miPushBll.Send(url, postParams, _appSecret);
            if (!string.IsNullOrEmpty(retString))
            {
                ret = JsonConvert.DeserializeObject<PushResultModel>(retString);
            }
            return ret;
        }

        /// <summary>
        /// 删除定时任务
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public PushResultModel ScheduleJobDelete(ScheduleJobModel messages)
        {
            var ret = new PushResultModel();
            if (messages == null || string.IsNullOrEmpty(_appSecret))
                return ret;

            const MiPushUrlEnum miPushUrlType = (MiPushUrlEnum)19;
            var urlChile = miPushUrlType.GetPushUrl();
            var url = Path.Combine(MiPushBaseUrl, urlChile);
            var postParams = GetProperties<ScheduleJobModel>(messages);
            var retString = _miPushBll.Send(url, postParams, _appSecret);
            if (!string.IsNullOrEmpty(retString))
            {
                ret = JsonConvert.DeserializeObject<PushResultModel>(retString);
            }
            return ret;
        }

        public string GetProperties<T>(T t)
        {
            string tStr = string.Empty;
            if (t == null)
            {
                return tStr;
            }
            System.Reflection.PropertyInfo[] properties = t.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

            if (properties.Length <= 0)
            {
                return tStr;
            }
            foreach (System.Reflection.PropertyInfo item in properties)
            {
                string name = item.Name;
                object value = item.GetValue(t, null);
                if (value != null && !name.Equals("_appSecret"))
                {
                    if (item.PropertyType.IsValueType || item.PropertyType.Name.StartsWith("String"))
                    {
                        if (item == properties.LastOrDefault())
                            tStr += string.Format("{0}={1}", name, value);
                        else
                            tStr += string.Format("{0}={1}&", name, value);
                    }
                    else
                    {
                        GetProperties(value);
                    }
                }
            }
            return tStr;
        }

        #region 暂未实现

        /// <summary>
        /// 删除别名
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        public bool DeleteAlia(string alias)
        {
            return false;
        }

        /// <summary>
        /// 删除标签
        /// </summary>
        /// <param name="tagName"></param>
        /// <returns></returns>
        public bool DeleteTag(string tagName)
        {
            return false;
        }

        #endregion

        #region 发送结果
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

        private PushResultModel Send(PushSendRegidModel messages)
        {
            var ret = new PushResultModel();
            if (messages == null || string.IsNullOrEmpty(_appSecret))
                return ret;
            var contentType = "application/x-www-form-urlencoded;";

            var miPushUrlType = (MiPushUrlEnum)1;
            var urlChile = miPushUrlType.GetPushUrl();
            var url = Path.Combine(MiPushBaseUrl, urlChile);

            if (messages != null)
            {
                if (!string.IsNullOrEmpty(messages.payload))
                    messages.payload = System.Web.HttpUtility.UrlEncode(messages.payload);
                var postParams = GetProperties<PushSendRegidModel>(messages);
                var retString = HttpUtil.HttpPost(url, postParams, _appSecret);
                if (!string.IsNullOrEmpty(retString))
                {
                    ret = JsonConvert.DeserializeObject<PushResultModel>(retString);
                }
            }

            return ret;
        }

        private PushResultModel Send(PushSendAliasModel messages)
        {
            var ret = new PushResultModel();
            if (messages == null || string.IsNullOrEmpty(_appSecret))
                return ret;

            const MiPushUrlEnum miPushUrlType = (MiPushUrlEnum)2;
            var urlChile = miPushUrlType.GetPushUrl();
            var url = Path.Combine(MiPushBaseUrl, urlChile);


            if (!string.IsNullOrEmpty(messages.payload))
                messages.payload = System.Web.HttpUtility.UrlEncode(messages.payload);
            var postParams = GetProperties<PushSendAliasModel>(messages);
            var retString = _miPushBll.Send(url, postParams, _appSecret);
            if (!string.IsNullOrEmpty(retString))
            {
                ret = JsonConvert.DeserializeObject<PushResultModel>(retString);
            }

            return ret;
        }


        private PushResultModel Send(PushSendAlliOSModel messages)
        {
            var ret = new PushResultModel();
            if (messages == null || string.IsNullOrEmpty(_appSecret))
                return ret;

            const MiPushUrlEnum miPushUrlType = (MiPushUrlEnum)5;
            var urlChile = miPushUrlType.GetPushUrl();
            var url = Path.Combine(MiPushBaseUrl, urlChile);

            var postParams = GetProperties<PushSendAlliOSModel>(messages);
            var retString = _miPushBll.Send(url, postParams, _appSecret);
            if (!string.IsNullOrEmpty(retString))
            {
                ret = JsonConvert.DeserializeObject<PushResultModel>(retString);
            }

            return ret;
        }

        /// <summary>
        /// 给不同的用户推送同一条消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="tokens"></param>
        private void SendUniformMessage(PushPluginMessage message, IEnumerable<TokenModel> tokens)
        {
            try
            {
                _log.Info($"MiPush插件开始执行一对多推送，数量: {tokens.Count()}");

                var pushMessage = new PushMessageModel()
                {
                    title = message.Title,
                    content = message.Message,
                    url = message.ClientActionUrl,
                    action = message.ClientAction
                };
                var sendData = new PushSendRegidModel()
                {
                    registration_id = string.Join(",", tokens.Select(p => p.Token)),
                    payload = JsonConvert.SerializeObject(pushMessage),
                    restricted_package_name = RestrictedPackageName,
                    pass_through = message.PassThrough,
                    title = message.Title,
                    description = message.Message,
                    notify_type = -1
                };
                var sentResult = Send(sendData);

                if (sentResult.result.Equals("ok"))
                {
                    foreach (var tokenModel in tokens)
                    {
                        PushSucess(tokenModel.BatchNo);
                    }
                }
                else
                {
                    foreach (var tokenModel in tokens)
                    {
                        PushFail(tokenModel.BatchNo);
                    }
                }

                _log.Info($"MiPush插件一对多推送发送完毕，数量: {tokens.Count()}");
            }
            catch (Exception ex)
            {
                _log.Error($"Android插件消息推送异常{ex}，数量：{tokens.Count()}");
            }
        }

        /// <summary>
        /// 给不同的用户推送不同的消息
        /// </summary>
        /// <param name="messages"></param>
        /// <returns></returns>
        private void SendDiffMessage(IEnumerable<PushPluginMessage> messages)
        {
            try
            {
                _log.Info($"MiPush插件开始执行多对多推送，数量: {messages.Count()}");

                foreach (var pushPluginMessage in messages)
                {
                    var pushMessage = new PushMessageModel()
                    {
                        title = pushPluginMessage.Title,
                        content = pushPluginMessage.Message,
                        url = pushPluginMessage.ClientActionUrl,
                        action = pushPluginMessage.ClientAction
                    };
                    var sendData = new PushSendRegidModel()
                    {
                        registration_id = string.Join(",", pushPluginMessage.Token),
                        payload = JsonConvert.SerializeObject(pushMessage),
                        restricted_package_name = RestrictedPackageName,
                        pass_through = pushPluginMessage.PassThrough,
                        title = pushPluginMessage.Title,
                        description = pushPluginMessage.Message,
                        notify_type = -1
                    };
                    var sentResult = Send(sendData);

                    if (sentResult.result.Equals("ok"))
                    {
                        PushSucess(pushPluginMessage.BatchNo);
                    }
                    else
                    {
                        PushFail(pushPluginMessage.BatchNo);
                    }
                }

                _log.Info($"MiPush插件多对多推送发送完毕，数量: {messages.Count()}");
            }
            catch (Exception ex)
            {
                _log.Error($"Android插件消息推送异常{ex}，数量：{messages.Count()}");
            }

        }
    }
}
