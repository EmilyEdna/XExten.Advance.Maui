using Android.App;
using Android.Views;
using XExten.Advance.Maui.MainActivity;

namespace XExten.Advance.Maui.Bar.Platforms.Android.Bar
{
    public class BarStatus: IBarStatus
    {
        private Activity Current => CrossCurrentActivity.Current.Activity;

        public void HiddenStatusBar()
        {
            Current.Window.AddFlags(WindowManagerFlags.Fullscreen);
        }
        public void ShowStatusBar()
        {
            Current.Window.ClearFlags(WindowManagerFlags.Fullscreen);
        }
    }
}
