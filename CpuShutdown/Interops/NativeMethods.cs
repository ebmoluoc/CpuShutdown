using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

namespace CpuShutdown.Interops
{
    internal static class NativeMethods
    {
        public const int ANYSIZE_ARRAY = 1;
        public const int GWL_STYLE = -16;
        public const int WM_SYSCOMMAND = 0x0112;
        public const uint SC_MAXIMIZE = 0xF030;
        public const uint SC_RESTORE = 0xF120;
        public const uint SC_SIZE = 0xF000;
        public const uint SC_CLOSE = 0xF060;
        public const uint MF_BYCOMMAND = 0x00000000;
        public const uint WS_MAXIMIZEBOX = 0x00010000;
        public const uint MSGFLT_ALLOW = 1;
        public const uint PROCESS_TERMINATE = 0x0001;
        public const uint CREATE_UNICODE_ENVIRONMENT = 0x00000400;
        public const uint ERROR_SUCCESS = 0;
        public const uint SHTDN_REASON_FLAG_PLANNED = 0x80000000;
        public const uint SHTDN_REASON_MAJOR_HARDWARE = 0x00010000;
        public const uint SHTDN_REASON_MINOR_PROCESSOR = 0x00000008;
        public const uint TOKEN_ADJUST_PRIVILEGES = 0x0020;
        public const uint TOKEN_QUERY = 0x0008;
        public const uint SE_PRIVILEGE_ENABLED = 0x00000002;
        public const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";
        public static readonly IntPtr HWND_BROADCAST = new IntPtr(0xffff);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool DeleteMenu([In] IntPtr hMenu, [In] uint uPosition, [In] uint uFlags);

        [DllImport("user32.dll")]
        public static extern IntPtr GetSystemMenu([In] IntPtr hWnd, [In] bool bRevert);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr GetWindowLongPtr([In] IntPtr hWnd, [In] int nIndex);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr SetWindowLongPtr([In] IntPtr hWnd, [In] int nIndex, [In] IntPtr dwNewLong);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool PostMessage([In] IntPtr hWnd, [In] int Msg, [In] IntPtr wParam, [In] IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr SendMessage([In] IntPtr hWnd, [In] int Msg, [In] IntPtr wParam, [In] IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int RegisterWindowMessage([In] string lpString);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool ChangeWindowMessageFilterEx([In] IntPtr hwnd, [In] int message, [In] uint action, /*_Inout_*/ IntPtr pChangeFilterStruct);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool OpenProcessToken([In] IntPtr ProcessHandle, [In] uint DesiredAccess, [Out] out IntPtr TokenHandle);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool LookupPrivilegeValue([In] string lpSystemName, [In] string lpName, /*_Out_*/ ref LUID lpLuid);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool AdjustTokenPrivileges([In] IntPtr TokenHandle, [In] bool DisableAllPrivileges, [In] ref TOKEN_PRIVILEGES NewState, [In] uint BufferLength, /*_Out_*/ IntPtr PreviousState, /*_Out_*/ IntPtr ReturnLength);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool InitiateSystemShutdownEx([In] string lpMachineName, [In] string lpMessage, [In] uint dwTimeout, [In] bool bForceAppsClosed, [In] bool bRebootAfterShutdown, [In] uint dwReason);

        //[DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        //public static extern bool AbortSystemShutdown([In] string lpMachineName);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool CreateProcessAsUser([In] IntPtr hToken, [In] string lpApplicationName, [In] string lpCommandLine, [In] IntPtr lpProcessAttributes, [In] IntPtr lpThreadAttributes, [In] bool bInheritHandles, [In] uint dwCreationFlags, [In] IntPtr lpEnvironment, [In] string lpCurrentDirectory, [In] ref STARTUPINFO lpStartupInfo, [Out] out PROCESS_INFORMATION lpProcessInformation);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern bool WTSQueryUserToken([In] uint SessionId, [Out] out IntPtr phToken);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle([In] IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CancelIoEx([In] SafePipeHandle hFile, [In] IntPtr lpOverlapped);

        [DllImport("kernel32.dll")]
        public static extern uint WTSGetActiveConsoleSessionId();

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetCurrentProcess();

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess([In] uint dwDesiredAccess, [In] bool bInheritHandle, [In] uint dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool TerminateProcess([In] IntPtr hProcess, [In] uint uExitCode);

        [DllImport("userenv.dll", SetLastError = true)]
        public static extern bool CreateEnvironmentBlock([Out] out IntPtr lpEnvironment, [In] IntPtr hToken, [In] bool bInherit);

        [DllImport("userenv.dll", SetLastError = true)]
        public static extern bool DestroyEnvironmentBlock([In] IntPtr lpEnvironment);
    }
}
