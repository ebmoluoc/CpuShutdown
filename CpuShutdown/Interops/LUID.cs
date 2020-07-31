using System.Runtime.InteropServices;

namespace CpuShutdown.Interops
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct LUID
    {
        public uint LowPart;
        public int HighPart;
    }
}
