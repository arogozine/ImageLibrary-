using System;
using System.Runtime.InteropServices;

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
 