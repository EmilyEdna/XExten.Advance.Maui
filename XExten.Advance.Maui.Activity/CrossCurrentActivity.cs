using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if ANDROID
using XExten.Advance.Maui.MainActivity.Platforms.Android;
#endif

namespace XExten.Advance.Maui.MainActivity
{
    public class CrossCurrentActivity
    {
#if ANDROID
        static Lazy<ICurrentActivity> implementation = new(CreateCurrentActivity, LazyThreadSafetyMode.PublicationOnly);

        public static ICurrentActivity Current
        {
            get
            {
                var ret = implementation.Value;
                if (ret == null)
                {
                    throw NotImplementedInReferenceAssembly();
                }
                return ret;
            }
        }

        static ICurrentActivity CreateCurrentActivity()
        {
            return new CurrentActivity();
        }
#endif
        internal static Exception NotImplementedInReferenceAssembly()
        {
            return new NotImplementedException("此功能未在此程序集的可移植版本中实现。 应从主应用程序项目引用 NuGet 包，以便引用特定于平台的实现。");
        }
    }
}
