using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ImageLibrary
{
    /// <summary>
    /// Native Method Calls
    /// </summary>
    internal static class NativeMethods
    {
        [DllImport("kernel32.dll")]
        internal static extern void RtlMoveMemory(IntPtr dest, IntPtr src, uint len);
    }
}
 