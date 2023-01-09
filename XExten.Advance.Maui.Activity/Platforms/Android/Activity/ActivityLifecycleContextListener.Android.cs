using Android.App;
using Android.Content;
using Android.OS;
using Application = Android.App.Application;
using Object = Java.Lang.Object;

namespace XExten.Advance.Maui.MainActivity.Platforms.Android
{
    internal class ActivityLifecycleContextListener : Object, Application.IActivityLifecycleCallbacks
    {
        WeakReference<Activity> currentActivity = new WeakReference<Activity>(null);
        public Context Context => Activity ?? Application.Context;
        CurrentActivity Current =>(CurrentActivity)(CrossCurrentActivity.Current);
        public Activity Activity
        {
            get => currentActivity.TryGetTarget(out Activity target) ? target : null;
            set => currentActivity.SetTarget(value);
        }
        public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
            Activity = activity;
            Current.RaiseStateChanged(activity, ActivityEventEnum.Created);
        }

        public void OnActivityDestroyed(Activity activity)
        {
            Current.RaiseStateChanged(activity, ActivityEventEnum.Destroyed);
        }

        public void OnActivityPaused(Activity activity)
        {
            Activity = activity;
            Current.RaiseStateChanged(activity, ActivityEventEnum.Paused);
        }

        public void OnActivityResumed(Activity activity)
        {
            Activity = activity;
            Current.RaiseStateChanged(activity, ActivityEventEnum.Resumed);
        }

        public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
        {
            Current.RaiseStateChanged(activity, ActivityEventEnum.SaveInstanceState);
        }

        public void OnActivityStarted(Activity activity)
        {
            Current.RaiseStateChanged(activity, ActivityEventEnum.Started);
        }

        public void OnActivityStopped(Activity activity)
        {
            Current.RaiseStateChanged(activity, ActivityEventEnum.Stopped);
        }
    }
}
