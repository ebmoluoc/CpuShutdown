using CpuShutdown.Interops;
using CpuShutdown.Settings;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace CpuShutdown.Services.UiSettings
{

    public sealed class UiSettings : IUiSettings, IDisposable
    {

        private readonly object _shownLock = new object();
        private Process _uiSettings;
        private bool _isDisposed;


        public void Dispose()
        {
            lock (_shownLock)
            {
                if (IsShown)
                {
                    _uiSettings.EnableRaisingEvents = false;
                    _uiSettings.Exited -= OnUiSettingsExited;

                    try
                    {
                        NativeMethods.SendMessage(_uiSettings.MainWindowHandle, NativeMethods.WM_SYSCOMMAND, (IntPtr)NativeMethods.SC_CLOSE, IntPtr.Zero);
                    }
                    catch (InvalidOperationException)
                    {
                        // MainWindowHandle is not defined because the process has exited
                    }
                    finally
                    {
                        _uiSettings.Dispose();
                        _uiSettings = null;

                        _isDisposed = true;
                    }
                }
            }
        }


        public bool IsShown
        {
            get
            {
                lock (_shownLock)
                {
                    return _uiSettings != null;
                }
            }
        }


        public void Show()
        {
            lock (_shownLock)
            {
                if (!_isDisposed)
                {
                    if (IsShown)
                    {
                        NativeMethods.PostMessage(NativeMethods.HWND_BROADCAST, AppSettings.WM_ACTIVATE_UI_SETTINGS, IntPtr.Zero, IntPtr.Zero);
                    }
                    else
                    {
                        _uiSettings = new Process();
                        _uiSettings.StartInfo.FileName = AppSettings.UiSettingsPath;
                        _uiSettings.StartInfo.Arguments = $"{AppSettings.ProjectGuidSwitch}{AppSettings.UiSettingsProjectGuid}";
                        _uiSettings.StartInfo.UseShellExecute = true;
                        _uiSettings.StartInfo.Verb = "runas";
                        _uiSettings.Exited += OnUiSettingsExited;
                        _uiSettings.EnableRaisingEvents = true;

                        try
                        {
                            _uiSettings.Start();
                        }
                        catch (Exception ex)
                        {
                            _uiSettings.Dispose();
                            _uiSettings = null;

                            // 1223 : runas was cancelled by user
                            if (!(ex is Win32Exception win32Exception && win32Exception.NativeErrorCode == 1223))
                                throw;
                        }
                    }
                }
            }
        }


        private void OnUiSettingsExited(object sender, EventArgs e)
        {
            lock (_shownLock)
            {
                _uiSettings?.Dispose();
                _uiSettings = null;
            }
        }

    }

}
