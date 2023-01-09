using Android.App;

namespace XExten.Advance.Maui.MainActivity.Platforms.Android
{
    public class ActivityEventArgs
    {
        internal ActivityEventArgs(Activity activity, ActivityEventEnum ev)
        {
            Event = ev;
            Activity = activity;
        }
        internal ActivityEventEnum Event { get; }
        internal Activity Activity { get; }
    }
}
