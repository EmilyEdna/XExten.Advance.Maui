using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XExten.Advance.Maui.Direction.Platforms.Android
{
    public enum OrientationEnum : uint
    {
        /// <summary>
        ///     Undefined or other orientation
        /// </summary>
        Undefined = 0U,

        /// <summary>
        ///     When rotate the device 0 degrees
        /// </summary>
        Portrait = 2U,

        /// <summary>
        ///     When rotate the device 90 degrees
        /// </summary>
        Landscape = 1U,

        /// <summary>
        ///     When rotate the device 180 degrees
        /// </summary>
        PortraitFlipped = 8U,

        /// <summary>
        ///     When rotate the device 270 degrees
        /// </summary>
        LandscapeFlipped = 4U
    }
}
