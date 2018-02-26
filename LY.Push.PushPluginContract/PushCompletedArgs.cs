namespace LY.Push.PushPluginContract
{
    public class PushCompletedArgs
    {
        /// <summary>
        /// 推送批次标识
        /// </summary>
        public string BatchNo
        {
            get; set;
        }


        public bool IsSuccess
        {
            get; set;
        }


    }
}