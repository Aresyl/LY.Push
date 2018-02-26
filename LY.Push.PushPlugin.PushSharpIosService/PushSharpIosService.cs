using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading;
using log4net;
using LY.Push.PushPlugin.PushSharpIosService.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PushSharp.Apple;

namespace LY.Push.PushPlugin.PushSharpIosService
{
    /// <summary>
    /// ios消息推送类
    /// </summary>
    public class PushSharpIosService : IDisposable
    {
        /// <summary>
        /// 创建一个推送对象
        /// </summary>
        private ApnsServiceBroker _apnsBroker;

        private ApnsConfiguration _config;

        /// <summary>
        /// 消息实体队列
        /// </summary>
        private List<iOSPushMessage> _messageList = new List<iOSPushMessage>();

        /// <summary>
        /// 日志记录
        /// </summary>
        private ILog _log = LogManager.GetLogger(typeof(PushSharpIosService));

        /// <summary>
        /// 证书路径
        /// </summary>
        private readonly string _appleCertpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["AppleCertPath"]);

        public event EventHandler<string> PushSucceeded;
        public event EventHandler<string> PushFailed;
        private static Timer _timer;
        private static readonly int StopTimerPeriod = 60000;

        /// <summary>
        /// 初始化
        /// </summary>
        public PushSharpIosService(string appleCertPwd, bool isTest)
        {
            var environment = ApnsConfiguration.ApnsServerEnvironment.Sandbox;

            if (!isTest)
            {
                environment = ApnsConfiguration.ApnsServerEnvironment.Production;
            }

            _config = new ApnsConfiguration(environment, _appleCertpath, appleCertPwd);

            if (_apnsBroker == null)
            {
                _apnsBroker = GetApnsBroker();
            }

        }

        private ApnsServiceBroker GetApnsBroker()
        {
            var apnsBroker = new ApnsServiceBroker(_config);

            apnsBroker.OnNotificationFailed += Broker_NotificationFailed;
            apnsBroker.OnNotificationSucceeded += Broker_NotificationSucceeded;

            return apnsBroker;
        }

        /// <summary>
        /// 添加需要推送的消息
        /// </summary>
        /// <param name="message">消息体</param>
        public void AddMessage(iOSPushMessage message)
        {
            _messageList.Add(message);
        }

        /// <summary>
        /// 推送消息(从队列中获取待推送消息)
        /// </summary>
        public void SendMessage()
        {
            //	var apnsBroker = GetApnsBroker();
            try
            {
                var appleCert = File.ReadAllBytes(_appleCertpath);
                if (appleCert.Length <= 0)
                {
                    return;
                }

                if (_messageList.Count <= 0)
                {
                    return;
                }

                _apnsBroker.Start();

                foreach (var item in _messageList)
                {
                    if (item.Token.Length == 64)
                    {
                        _apnsBroker.QueueNotification(new ApnsNotification(item.Token, JObject.Parse(item.Body)));
                    }
                }

                //推送成功,清除队列
                _messageList.Clear();
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("消息推送异常：{0}", ex);
            }
            finally
            {
                if (_apnsBroker.IsCompleted)
                {
                    //消息推送完毕
                    _apnsBroker.Stop();
                    _log.Info($"该批iOS端推送共{_messageList.Count}条，已完成请求");
                }

                // 设置定时任务，每1分钟检查请求完成情况
                SetTimer(StopTimerPeriod);

            }
        }

        /// <summary>
        /// 推送消息
        /// </summary>
        public void SendMessage(List<iOSPushMessage> listMessage)
        {
            //var apnsBroker = GetApnsBroker();
            try
            {
                _log.Info($"iOS插件开始执行推送，数量: {listMessage.Count}");

                var appleCert = File.ReadAllBytes(_appleCertpath);
                if (appleCert.Length <= 0)
                {
                    return;
                }

                if (listMessage.Count <= 0)
                {
                    return;
                }

                // 开始推送
                _apnsBroker.Start();

                foreach (var item in listMessage)
                {
                    var send = new SendIosModel
                    {
                        action = JsonConvert.DeserializeObject<ActionModel>(item.Message),
                        aps = new ApsModel { alert = item.Body, badge = 1, sound = "pulse.mp3" }
                    };
                    _apnsBroker.QueueNotification(new ApnsNotification(item.Token, JObject.Parse(JsonConvert.SerializeObject(send))) { Tag = new { BatchNo = item.BatchNo, IsUserToken = item.IsUserToken } });

                }
                _log.Info($"iOS插件推送数据发送完毕，数量: {listMessage.Count}");
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("iOS插件消息推送异常：{0}", ex);
            }
            finally
            {
                if (_apnsBroker.IsCompleted)
                {
                    //消息推送完毕
                    _apnsBroker.Stop();
                    _log.Info($"该批iOS推送共{listMessage.Count}条，已结束请求");
                }

                // 设置定时任务，每1分钟检查请求完成情况
                SetTimer(StopTimerPeriod);
            }
        }

        #region ios推送相关事件

        /// <summary>
        /// 推送成功
        /// </summary>
        /// <param name="notification"></param>
        private void Broker_NotificationSucceeded(ApnsNotification notification)
        {
            try
            {
                if (notification.Tag != null)
                {
                    var result = (dynamic)notification.Tag;
                    if (result != null && result.BatchNo != null)
                    {
                        if (!result.BatchNo.Equals("0"))
                        {
                            var handler = PushSucceeded;
                            if (handler != null)
                            {
                                handler(this, result.BatchNo);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("回调失败：{0},{1}", ex, JsonConvert.SerializeObject(notification));
            }

#if DEBUG
            _log.DebugFormat("IOS消息推送成功：Token：{0}, Tag:{1}", notification.DeviceToken, notification.Tag);
#endif
        }

        /// <summary>
        /// 推送失败
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="aggregateEx"></param>
        private void Broker_NotificationFailed(ApnsNotification notification, AggregateException aggregateEx)
        {
#if DEBUG
            _log.DebugFormat("【iOS发送推送】失败 回调{0},{1}", notification.DeviceToken, aggregateEx.Message);
#endif
            //aggregateEx.Handle(ex =>
            //{
            //	var notificationException = ex as ApnsNotificationException;
            //	if (notificationException != null)
            //	{
            //		_log.Error($"IOS推送失败: Token:{notification.DeviceToken}, Tag: {notification.Tag};{Environment.NewLine}" +
            //                            $"Notification：{notificationException.Notification};{Environment.NewLine}" +
            //		           $"Exception：{aggregateEx}; StatusCode:{notificationException.ErrorStatusCode}");
            //	}
            //	else
            //	{
            //		// Inner exception might hold more useful information like an ApnsConnectionException           
            //		_log.ErrorFormat("IOS推送失败：Exception：{0}", aggregateEx.InnerExceptions);
            //	}

            //	// Mark it as handled
            //	return true;
            //});

            aggregateEx.Handle(ex =>
            {
                _log.Error($"IOS推送失败: Token:{notification.DeviceToken}, Tag: {notification.Tag};{Environment.NewLine}" +
                           $"Exception：{ex.Message}; ");

                // Mark it as handled
                return true;
            });

            //回调
            try
            {
                if (notification.Tag != null)
                {
                    var result = (dynamic)notification.Tag;
                    if (result != null && result.BatchNo != null)
                    {
                        if (!result.BatchNo.Equals("0"))
                        {
                            var handler = PushFailed;
                            if (handler != null)
                            {
                                handler(this, result.BatchNo);
                            }
                        }
                    }
                }
            }
            catch (Exception exs)
            {
                _log.ErrorFormat("回调失败：{0},{1}", exs, JsonConvert.SerializeObject(notification));
            }
#if DEBUG
            _log.DebugFormat("IOS消息推送失败：Notification：{0}", notification);
#endif

        }

        #endregion

        /// <summary>
        /// 设置定时任务检查推送请求发送情况
        /// </summary>
        private void SetTimer(int stopTimerPeriod)
        {
            if (_timer == null)
            {
                _timer = new Timer(StopTimerCallback, null, stopTimerPeriod, Timeout.Infinite);
            }
        }

        /// <summary>
        /// 推送请求发送情况定时检查任务
        /// </summary>
        /// <param name="data"></param>
        private void StopTimerCallback(object data)
        {
            // 尝试关闭请求代理
            if (_apnsBroker.IsCompleted)
            {
                _apnsBroker.Stop();
                _log.Info("当前活跃中的iOS端推送会话已完成请求");
            }

            // 一段时间后再次执行
            _timer.Change(StopTimerPeriod, Timeout.Infinite);
        }

        public void Dispose()
        {
            if (_apnsBroker != null)
            {
                _apnsBroker.Stop(true);
            }
        }

    }
}