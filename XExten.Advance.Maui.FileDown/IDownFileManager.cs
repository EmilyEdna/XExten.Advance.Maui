using XExten.Advance.Maui.FileDown.Platforms.Android;

namespace XExten.Advance.Maui.FileDown
{
    public interface IDownFileManager
    {
#if ANDROID
        private static Lazy<IDownManager> Implementation = new Lazy<IDownManager>(()=>new DownManager(), LazyThreadSafetyMode.PublicationOnly);
        public static IDownManager Current
        {
            get
            {
                var ret = Implementation.Value;
                if (ret == null)
                {
                    throw new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
                }
                return ret;
            }
        }
#endif
    }
}