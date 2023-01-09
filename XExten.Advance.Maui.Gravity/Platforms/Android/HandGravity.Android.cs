using Android.Content;
using Android.Hardware;
using XExten.Advance.Maui.MainActivity;

namespace XExten.Advance.Maui.Gravity.Platforms.Android
{
    public class HandGravity: IHandGravity
    {
        public static SensorManager Manager { get; set; }
        public static SensorEventListener Listener = new SensorEventListener();
        public void RegistEvent()
        {
            Manager ??= (SensorManager)CrossCurrentActivity.Current.Activity.GetSystemService(Context.SensorService);
            var sensor = Manager.GetDefaultSensor(SensorType.Accelerometer);
            Manager.RegisterListener(Listener, sensor, SensorDelay.Normal);
        }
        public void UnRegistEvent()
        {
            if (Manager != null)
                Manager.UnregisterListener(Listener);
        }
    }
}
