using Android.Content;
using Android.Hardware;
using Android.Runtime;
using Android.Views;
using Application = Android.App.Application;

namespace XExten.Advance.Maui.Direction.Platforms.Android
{
    public class OrientationListener : OrientationEventListener
    {
        readonly Action<OrientationChanged> OrientationChanged;

        OrientationEnum CachedOrientation;
        public OrientationListener(Action<OrientationChanged> OrientationChanged) : base(Application.Context, SensorDelay.Normal)
        {
            this.OrientationChanged = OrientationChanged;
        }
        public OrientationListener(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }
        public OrientationListener(Context context) : base(context) { }
        public OrientationListener(Context context, SensorDelay rate) : base(context, rate) { }
        public override void OnOrientationChanged(int orientation)
        {
            var currentOrientation = Direction.Current.CurrentOrientation;

            if (currentOrientation != CachedOrientation)
            {
                CachedOrientation = currentOrientation;

                OrientationChanged(new OrientationChanged
                {
                    Orientation = currentOrientation
                });
            }
        }
    }
}
