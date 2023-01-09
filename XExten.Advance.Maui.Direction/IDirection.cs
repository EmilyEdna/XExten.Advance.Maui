namespace XExten.Advance.Maui.Direction
{
    public interface IDirection
    {
#if ANDROID
        static Platforms.Android.IDeviceOrientation Instance => Platforms.Android.Direction.Current;
#endif
    }
}
