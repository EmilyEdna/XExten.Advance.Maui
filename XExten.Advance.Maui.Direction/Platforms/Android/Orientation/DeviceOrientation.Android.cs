using Android.Content.PM;
using Android.Views;
using XExten.Advance.Maui.MainActivity;
using Orientation = Android.Content.Res.Orientation;

namespace XExten.Advance.Maui.Direction.Platforms.Android
{
    public class DeviceOrientation : BaseDeviceOrientation
    {
        readonly OrientationListener _listener;
        bool _disposed;
        bool _isListenerEnabled = true;

        protected bool IsListenerEnabled
        {
            set
            {
                if (_listener == null) return;

                if (value == _isListenerEnabled) return;

                if (value)
                {
                    _listener.Enable();
                }
                else
                {
                    _listener.Disable();
                }

                _isListenerEnabled = value;
            }
        }

        public DeviceOrientation()
        {
            _listener = new OrientationListener(OnOrientationChanged);

            if (_listener.CanDetectOrientation())
            {
                _listener.Enable();
            }
        }

        public override OrientationEnum CurrentOrientation
        {
            get
            {
                var activity = CrossCurrentActivity.Current.Activity;
                var rotation = activity.WindowManager.DefaultDisplay.Rotation;
                return Convert(rotation);
            }
        }

        public override void LockOrientation(OrientationEnum orientation)
        {
            var activity = CrossCurrentActivity.Current.Activity;

            activity.RequestedOrientation = Convert(orientation);
        }

        public override void UnlockOrientation()
        {
            var activity = CrossCurrentActivity.Current.Activity;

            activity.RequestedOrientation = Convert(OrientationEnum.Undefined);
        }

        public override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing && _listener != null)
                {
                    _listener.Disable();
                    _listener.Dispose();
                }

                _disposed = true;
            }

            base.Dispose(disposing);
        }

        public static void NotifyOrientationChange(Orientation newOrientation, bool isForms = true)
        {
            var instance = (DeviceOrientation)Direction.Current;

            if (instance == null)
                throw new InvalidCastException("Cast from IDeviceOrientation to Android.DeviceOrientationImplementation");

            instance.IsListenerEnabled = !isForms;

            instance.OnOrientationChanged(new OrientationChanged
            {
                Orientation = Direction.Current.CurrentOrientation
            });
        }

        ScreenOrientation Convert(OrientationEnum orientation)
        {
            switch (orientation)
            {
                case OrientationEnum.Portrait:
                    return ScreenOrientation.Portrait;
                case OrientationEnum.PortraitFlipped:
                    return ScreenOrientation.ReversePortrait;
                case OrientationEnum.Landscape:
                    return ScreenOrientation.Landscape;
                case OrientationEnum.LandscapeFlipped:
                    return ScreenOrientation.ReverseLandscape;
                default:
                    return ScreenOrientation.Unspecified;
            }
        }

        OrientationEnum Convert(SurfaceOrientation orientation)
        {
            switch (orientation)
            {
                case SurfaceOrientation.Rotation0:
                    return OrientationEnum.Portrait;
                case SurfaceOrientation.Rotation180:
                    return OrientationEnum.PortraitFlipped;
                case SurfaceOrientation.Rotation90:
                    return OrientationEnum.Landscape;
                case SurfaceOrientation.Rotation270:
                    return OrientationEnum.LandscapeFlipped;
                default:
                    return OrientationEnum.Undefined;
            }
        }
    }
}
