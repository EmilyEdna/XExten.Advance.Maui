using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XExten.Advance.Maui.Direction.Platforms.Android
{
    public abstract class BaseDeviceOrientation : IDeviceOrientation, IDisposable
    {
        bool _disposed;

        public event OrientationHandler.OrientationChangedEventHandler OrientationChanged;

        ~BaseDeviceOrientation()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Dispose method
        /// </summary>
        /// <param name="disposing"></param>
        public virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
            }
        }

        ///     When orientation changes
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnOrientationChanged(OrientationChanged e)
        {
            OrientationChanged?.Invoke(this, e);
        }

        /// <summary>
        ///     Current device orientation
        /// </summary>
        public abstract OrientationEnum CurrentOrientation { get; }

        /// <summary>
        ///     Lock orientation in the specified position
        /// </summary>
        /// <param name="orientation">Position for lock.</param>
        public abstract void LockOrientation(OrientationEnum orientation);

        /// <summary>
        ///     Unlock orientation
        /// </summary>
        public abstract void UnlockOrientation();
    }
}
