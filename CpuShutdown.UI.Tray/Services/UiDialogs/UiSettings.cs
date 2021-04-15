using CpuShutdown.Settings;
using CpuShutdown.UI.Tray.Interops;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace CpuShutdown.UI.Tray.Services.UiDialogs
{

    public sealed class UiSettings : IUiDialog, IDisposable
    {

        private readonly object _shown = new();
        private Process _process;
        private bool _disposed;


        public void Dispose()
        {
            lock (_shown)
            {
                if (IsShown)
                {
                    _process.EnableRaisingEvents = false;
                    _process.Exited -= OnExited;

                    try
                    {
                        NativeMethods.SendMessage(_process.MainWindowHandle, NativeMethods.WM_SYSCOMMAND, (IntPtr)NativeMethods.SC_CLOSE, IntPtr.Zero);
                    }
                    catch (InvalidOperationException)
                    {
                        // MainWindowHandle is not defined because the process has exited
                    }
                    finally
                    {
                        _process.Dispose();
                        _process = null;
                    }
                }

                _disposed = true;
            }
        }


        public bool IsShown
        {
            get
            {
                lock (_shown)
                {
                    return _process != null;
                }
            }
        }


        public void Show()
        {
            lock (_shown)
            {
                if (!_disposed)
                {
                    if (IsShown)
                    {
                        NativeMethods.PostMessage(NativeMethods.HWND_BROADCAST, AppSettings.WM_ACTIVATE_UI_SETTINGS, IntPtr.Zero, IntPtr.Zero);
                    }
                    else
                    {
                        _process = new Process();
                        _process.StartInfo.FileName = AppSettings.UiSettingsPath;
                        _process.StartInfo.UseShellExecute = true;
                        _process.StartInfo.Verb = "runas";
                        _process.Exited += OnExited;
                        _process.EnableRaisingEvents = true;

                        try
                        {
                            _process.Start();
                        }
                        catch (Exception ex)
                        {
                            _process.Dispose();
                            _process = null;

                            // runas was cancelled by user
                            var win32Exception = ex as Win32Exception;
                            if (win32Exception?.NativeErrorCode != NativeMethods.ERROR_CANCELLED)
                                throw;
                        }
                    }
                }
            }
        }


        private void OnExited(object sender, EventArgs e)
        {
            lock (_shown)
            {
                _process?.Dispose();
                _process = null;
            }
        }

    }

}
