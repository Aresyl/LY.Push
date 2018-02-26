using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace YL.Push.PushPlugin.MiPush
{
    public class HttpUtil
    {
        /// <summary>
        /// 发生post请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postParams">Post请求参数</param>
        /// <param name="contentType">请求内容类型（格式：application/x-www-form-urlencoded）</param>
        /// <returns></returns>
        public static string HttpPost(string url, string postParams, string authorization, string contentType = "application/x-www-form-urlencoded")
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = contentType;
            if (!string.IsNullOrEmpty(authorization))
                request.Headers.Add("Authorization", string.Format("key={0}", authorization));
            //request.Headers.Add("Authorization", "key=SoUvSCQKDxdAX56ltpL3Tg==");
            request.Timeout = 120000;
            request.ReadWriteTimeout = 120000;
            request.KeepAlive = false;
            request.ContinueTimeout = 0;
            //request.ContentLength = Encoding.UTF8.GetByteCount(postParams);

            HttpWebResponse response = null;
            string responseDatas = string.Empty;
            try
            {
                byte[] postBytes = Encoding.UTF8.GetBytes(postParams);
                using (var requestJons = request.GetRequestStream())
                    requestJons.Write(postBytes, 0, postBytes.Length);

                response = (HttpWebResponse)request.GetResponse();
                Stream streamResponse = response.GetResponseStream();
                if (streamResponse != null)
                {
                    using (StreamReader sr = new StreamReader(streamResponse, Encoding.UTF8))
                    {
                        responseDatas = sr.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                request.Abort();
                if (ex.InnerException != null)
                    throw new Exception(ex.InnerException.Message);
                throw new Exception(ex.Message);
            }
            finally
            {
                if (response != null)
                {
                    try
                    {
                        response.Close();
                    }
                    catch
                    {
                        request.Abort();
                    }
                }
                else
                {
                    request.Abort();
                }
            }
            return responseDatas;
        }

        /// <summary>
        /// 发生get请求
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="postDataStr"></param>
        /// <returns></returns>
        public static string HttpGet(string Url, string postDataStr)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }

        /// <summary>
        /// 给Url添加参数
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="dictParam">参数字典</param>
        /// <returns>带参数的Url</returns>
        public static string AddUrlParam(string url, Dictionary<string, string> dictParam)
        {
            //构造url
            if (dictParam != null && dictParam.Count > 0)
            {
                int index = url.IndexOf("?");
                if (index == -1)
                {
                    url += "?";
                }
                else if (index != url.Length - 1)//说明现有url已带参数
                {
                    url += "&";
                }
                foreach (var pair in dictParam)
                {
                    url += pair.Key + "=" + pair.Value + "&";
                }
                url = url.TrimEnd('&');
            }
            return url;
        }

        /// <summary>
        /// 将字符串进行urlencode
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UrlEncode(string str)
        {
            StringBuilder sb = new StringBuilder();
            byte[] byStr = System.Text.Encoding.UTF8.GetBytes(str);
            for (int i = 0; i < byStr.Length; i++)
            {
                sb.Append(@"%" + Convert.ToString(byStr[i], 16));
            }

            return (sb.ToString());
        }

        ///// <summary>
        ///// 获取外网IP地址
        ///// </summary>
        ///// <returns></returns>
        //public static string GetIP()
        //{
        //    string userIP;
        //    HttpRequest rq = HttpContext.Current.Request;
        //    // 如果使用代理，获取真实IP   
        //    if (rq.ServerVariables["HTTP_X_FORWARDED_FOR"] != "")
        //        userIP = rq.ServerVariables["REMOTE_ADDR"];
        //    else
        //        userIP = rq.ServerVariables["HTTP_X_FORWARDED_FOR"];
        //    if (string.IsNullOrEmpty(userIP))
        //        userIP = rq.UserHostAddress;
        //    return userIP;
        //}

        /// <summary>
        /// Url参数拼接
        /// </summary>
        /// <param name="dictParam">参数字典</param>
        /// <returns>拼接的参数</returns>
        public static string GetPostDataParam(Dictionary<string, string> dictParam)
        {
            var postData = "";
            //参数字典
            if (dictParam != null && dictParam.Count > 0)
            {

                foreach (var pair in dictParam)
                {
                    postData += pair.Key + "=" + pair.Value + "&";
                }
                postData = postData.TrimEnd('&');
            }
            return postData;
        }
    }
}