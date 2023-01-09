using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XExten.Advance.Maui.Direction.Platforms.Android
{
    public interface IDeviceOrientation
    {
        /// <summary>
        ///     Gets current device orientation
        /// </summary>
        OrientationEnum CurrentOrientation { get; }

        /// <summary>
        ///     Event handler when orientation changes
        /// </summary>
        event OrientationHandler.OrientationChangedEventHandler OrientationChanged;

        /// <summary>
        ///     Lock orientation in the specified position
        /// </summary>
        /// <param name="orientation">Position for lock.</param>
        void LockOrientation(OrientationEnum orientation);

        /// <summary>
        ///     Unlock orientation
        /// </summary>
        void UnlockOrientation();
    }
}
