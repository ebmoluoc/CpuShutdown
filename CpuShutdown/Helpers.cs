using CpuShutdown.Interops;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Threading;

namespace CpuShutdown
{

    public static class Helpers
    {

        public static string StringFromEmbeddedResource(Assembly assembly, string resourceName)
        {
            var name = $"{assembly.GetName().Name}.{resourceName}";
            var stream = assembly.GetManifestResourceStream(name);

            using var sr = new StreamReader(stream);
            return sr.ReadToEnd();
        }


        public static Mutex CreateOwnedMutex(string name)
        {
            var mutex = new Mutex(true, name, out var createdNew);
            if (!createdNew)
            {
                mutex.Dispose();
                throw new InvalidOperationException("Mutex already owned");
            }

            return mutex;
        }


        public static void RestartService(string name)
        {
            using var service = new ServiceController(name);
            service.Stop();
            service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(2));
            service.Start();
        }


        public static uint GetActiveSessionId()
        {
            return NativeMethods.WTSGetActiveConsoleSessionId();
        }


        public static bool IsUserLogged(uint sessionId)
        {
            var success = NativeMethods.WTSQueryUserToken(sessionId, out var hToken);
            if (success)
                NativeMethods.CloseHandle(hToken);

            return success;
        }


        public static uint CreateProcessAsUser(string applicationName, string commandLine, uint sessionId)
        {
            if (!NativeMethods.WTSQueryUserToken(sessionId, out var hToken))
            {
                var exception = new Win32Exception(Marshal.GetLastWin32Error());
                throw exception;
            }

            if (!NativeMethods.CreateEnvironmentBlock(out var lpEnvironment, hToken, false))
            {
                var exception = new Win32Exception(Marshal.GetLastWin32Error());
                NativeMethods.CloseHandle(hToken);
                throw exception;
            }

            var startupInfo = new STARTUPINFO { cb = Marshal.SizeOf(typeof(STARTUPINFO)), lpDesktop = "winsta0\\default" };
            if (!NativeMethods.CreateProcessAsUser(hToken, applicationName, commandLine, IntPtr.Zero, IntPtr.Zero, true, NativeMethods.CREATE_UNICODE_ENVIRONMENT, lpEnvironment, null, ref startupInfo, out var processInfo))
            {
                var exception = new Win32Exception(Marshal.GetLastWin32Error());
                NativeMethods.CloseHandle(hToken);
                NativeMethods.DestroyEnvironmentBlock(lpEnvironment);
                throw exception;
            }

            NativeMethods.CloseHandle(hToken);
            NativeMethods.CloseHandle(processInfo.hThread);
            NativeMethods.CloseHandle(processInfo.hProcess);
            NativeMethods.DestroyEnvironmentBlock(lpEnvironment);

            return processInfo.dwProcessId;
        }


        public static void TerminateProcess(uint processId)
        {
            var hProcess = NativeMethods.OpenProcess(NativeMethods.PROCESS_TERMINATE, false, processId);
            if (hProcess != IntPtr.Zero)
            {
                NativeMethods.TerminateProcess(hProcess, 1);
                NativeMethods.CloseHandle(hProcess);
            }
        }


        public static void AllowWindowMessage(IntPtr windowHandle, int windowMessage)
        {
            NativeMethods.ChangeWindowMessageFilterEx(windowHandle, windowMessage, NativeMethods.MSGFLT_ALLOW, IntPtr.Zero);
        }


        public static void RemoveSystemMenuMaximize(IntPtr windowHandle)
        {
            var systemMenuHandle = NativeMethods.GetSystemMenu(windowHandle, false);

            RemoveWindowStyle(windowHandle, NativeMethods.GWL_STYLE, NativeMethods.WS_MAXIMIZEBOX);
            NativeMethods.DeleteMenu(systemMenuHandle, NativeMethods.SC_MAXIMIZE, NativeMethods.MF_BYCOMMAND);
            NativeMethods.DeleteMenu(systemMenuHandle, NativeMethods.SC_RESTORE, NativeMethods.MF_BYCOMMAND);
        }


        public static void RemoveSystemMenuSize(IntPtr windowHandle)
        {
            var systemMenuHandle = NativeMethods.GetSystemMenu(windowHandle, false);

            NativeMethods.DeleteMenu(systemMenuHandle, NativeMethods.SC_SIZE, NativeMethods.MF_BYCOMMAND);
        }


        public static void SystemShutdown(string message, int timeout)
        {
            if (EnableShutdownPrivilege(NativeMethods.GetCurrentProcess()))
                if (NativeMethods.InitiateSystemShutdownEx(null, message, (uint)timeout, true, false, NativeMethods.SHTDN_REASON_FLAG_PLANNED | NativeMethods.SHTDN_REASON_MAJOR_HARDWARE | NativeMethods.SHTDN_REASON_MINOR_PROCESSOR))
                    return;

            // as a backup in case the other way fails
            Process.Start("shutdown.exe", $"/s /t {timeout} /c \"{message}\"");
        }


        //public static void AbortSystemShutdown()
        //{
        //    if (EnableShutdownPrivilege(NativeMethods.GetCurrentProcess()))
        //        if (NativeMethods.AbortSystemShutdown(null))
        //            return;

        //    // as a backup in case the other way fails
        //    Process.Start("shutdown.exe", $"/a");
        //}


        private static bool EnableShutdownPrivilege(IntPtr hProcess)
        {
            var success = false;

            if (NativeMethods.OpenProcessToken(hProcess, NativeMethods.TOKEN_ADJUST_PRIVILEGES | NativeMethods.TOKEN_QUERY, out var hToken))
            {
                var tokenPrivileges = new TOKEN_PRIVILEGES { PrivilegeCount = 1, Privileges = new LUID_AND_ATTRIBUTES[1] };
                tokenPrivileges.Privileges[0].Attributes = NativeMethods.SE_PRIVILEGE_ENABLED;

                if (NativeMethods.LookupPrivilegeValue(null, NativeMethods.SE_SHUTDOWN_NAME, ref tokenPrivileges.Privileges[0].Luid))
                    if (NativeMethods.AdjustTokenPrivileges(hToken, false, ref tokenPrivileges, 0, IntPtr.Zero, IntPtr.Zero))
                        success = Marshal.GetLastWin32Error() == NativeMethods.ERROR_SUCCESS;
            }

            return success;
        }


        private static bool RemoveWindowStyle(IntPtr hWnd, int index, uint style)
        {
            var success = false;

            var value = (uint)NativeMethods.GetWindowLongPtr(hWnd, index);
            if (value != 0)
            {
                value &= ~style;
                success = NativeMethods.SetWindowLongPtr(hWnd, index, (IntPtr)value) != IntPtr.Zero;
            }

            return success;
        }

    }

}
