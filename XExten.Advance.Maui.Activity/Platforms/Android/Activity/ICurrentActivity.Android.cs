﻿using Android.App;
using Android.Content;
using Android.OS;
using Application = Android.App.Application;

namespace XExten.Advance.Maui.MainActivity.Platforms.Android
{
    public interface ICurrentActivity
    {
        /// <summary>
        /// Gets or sets the activity.
        /// </summary>
        /// <value>The activity.</value>
        Activity Activity { get; set; }

        /// <summary>
        /// Gets the current Application Context.
        /// </summary>
        /// <value>The app context.</value>
        Context AppContext { get; }

        /// <summary>
        /// Fires when activity state events are fired
        /// </summary>
        event EventHandler<ActivityEventArgs> ActivityStateChanged;

        /// <summary>
        /// Waits for an activity to be ready for use
        /// </summary>
        /// <returns></returns>
        Task<Activity> WaitForActivityAsync(CancellationToken cancelToken = default);

        /// <summary>
        /// Initialize Current Activity Plugin with Application
        /// </summary>
        /// <param name="application"></param>
        void Init(Application application);

        /// <summary>
        /// Initialize the current activity with activity and bundle
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="bundle"></param>
        void Init(Activity activity, Bundle bundle);
    }

}
