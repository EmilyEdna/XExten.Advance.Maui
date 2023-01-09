using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XExten.Advance.Maui.Direction.Platforms.Android
{
    public class Direction: IDirection
    {
        private static readonly Lazy<IDeviceOrientation> Implementation =new Lazy<IDeviceOrientation>(() =>new DeviceOrientation(), LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        ///     Current settings to use
        /// </summary>
        public static IDeviceOrientation Current => Implementation.Value;
    }
}
