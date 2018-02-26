namespace YL.Push.PushPlugin.MiPush
{
    public class MiPushBll
    {
        public string Send(string miPushUrl, string postParams, string authorization)
        {
            var ret = "";
            ret = HttpUtil.HttpPost(miPushUrl, postParams, authorization);
            return ret;
        }
    }
}