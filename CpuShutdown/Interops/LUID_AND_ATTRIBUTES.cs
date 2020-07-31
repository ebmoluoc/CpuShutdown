using System.Runtime.InteropServices;

namespace CpuShutdown.Interops
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct LUID_AND_ATTRIBUTES
    {
        public LUID Luid;
        public uint Attributes;
    }
}
