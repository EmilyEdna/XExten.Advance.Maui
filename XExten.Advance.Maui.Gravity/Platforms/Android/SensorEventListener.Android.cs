using Android.Hardware;
using Android.Runtime;
using XExten.Advance.Maui.Direction.Platforms.Android;
using Object = Java.Lang.Object;

namespace XExten.Advance.Maui.Gravity
{
    // All the code in this file is only included on Android.
    public class SensorEventListener : Object, ISensorEventListener
    {
        private OrientationEnum Orientation = OrientationEnum.Portrait;
        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {
        }

        public void OnSensorChanged(SensorEvent e)
        {
            if (SensorType.Accelerometer != e.Sensor.Type) return;

            float x = e.Values[0];
            float y = e.Values[1];


            OrientationEnum NewOrientation;

            if (x < 4.5 && x >= -4.5 && y >= 4.5)
                NewOrientation = OrientationEnum.Portrait;
            else if (x >= 4.5 && y < 4.5 && y >= -4.5)
                NewOrientation = OrientationEnum.Landscape;
            else if (x <= -4.5 && y < 4.5 && y >= -4.5)
                NewOrientation = OrientationEnum.LandscapeFlipped;
            else
                NewOrientation = OrientationEnum.PortraitFlipped;
            if (Orientation != NewOrientation)
            {
                Direction.Platforms.Android.Direction.Current.LockOrientation(NewOrientation);
                Orientation = NewOrientation;
            }

        }
    }
}