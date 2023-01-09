namespace XExten.Advance.Maui.Bar
{
    public interface IBarStatus
    {
        /// <summary>
        /// 隐藏通知栏
        /// </summary>
        void HiddenStatusBar();
        /// <summary>
        /// 显示通知栏
        /// </summary>
        void ShowStatusBar();
        static IBarStatus Instance => new Lazy<IBarStatus>(() =>
        {
#if ANDROID
     return new Platforms.Android.Bar.BarStatus();
#else
     return null;
#endif
        }, LazyThreadSafetyMode.PublicationOnly).Value;
    }
}
