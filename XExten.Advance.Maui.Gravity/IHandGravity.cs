namespace XExten.Advance.Maui.Gravity
{
    public interface IHandGravity
    {
        static IHandGravity Instance => new Lazy<IHandGravity>(() =>
        {
#if ANDROID
            return new Platforms.Android.HandGravity();
#else
            return null;
#endif
        }, LazyThreadSafetyMode.PublicationOnly).Value;

        void RegistEvent();
        void UnRegistEvent();
    }
}