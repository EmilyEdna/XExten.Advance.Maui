using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Application = Android.App.Application;

namespace XExten.Advance.Maui.MainActivity.Platforms.Android
{
    [Preserve(AllMembers = true)]
    public class CurrentActivity : ICurrentActivity
    {
        ActivityLifecycleContextListener lifecycleListener;
        public Activity Activity {
            get => lifecycleListener?.Activity;
            set
            {
                if (lifecycleListener == null)
                    Init(value, null);
            }
        }
        public Context AppContext =>Application.Context;

        public event EventHandler<ActivityEventArgs> ActivityStateChanged;

        internal void RaiseStateChanged(Activity activity, ActivityEventEnum ev) => ActivityStateChanged?.Invoke(this, new ActivityEventArgs(activity, ev));

        public void Init(Application application)
        {
            if (lifecycleListener != null)
                return;

            lifecycleListener = new ActivityLifecycleContextListener();
            application.RegisterActivityLifecycleCallbacks(lifecycleListener);
        }

        public void Init(Activity activity, Bundle bundle)
        {
            Init(activity.Application);
            lifecycleListener.Activity = activity;
        }

        public async Task<Activity> WaitForActivityAsync(CancellationToken cancelToken = default)
        {
            if (Activity != null)
                return Activity;

            var tcs = new TaskCompletionSource<Activity>();
            var handler = new EventHandler<ActivityEventArgs>((sender, args) =>
            {
                if (args.Event == ActivityEventEnum.Created || args.Event == ActivityEventEnum.Resumed)
                    tcs.TrySetResult(args.Activity);
            });

            try
            {
                using (cancelToken.Register(() => tcs.TrySetCanceled()))
                {
                    ActivityStateChanged += handler;
                    return await tcs.Task.ConfigureAwait(false);
                }
            }
            finally
            {
                ActivityStateChanged -= handler;
            }
        }
    }
}
