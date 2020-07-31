using System.Runtime.InteropServices;

namespace CpuShutdown.Interops
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct TOKEN_PRIVILEGES
    {
        public uint PrivilegeCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NativeMethods.ANYSIZE_ARRAY)]
        public LUID_AND_ATTRIBUTES[] Privileges;
    }
}
