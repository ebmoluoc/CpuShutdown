using System;
using System.Windows.Forms;

namespace CpuShutdown.UI.Tray.Services.UiDialogs
{

    public sealed class UiAbout : IUiDialog, IDisposable
    {

        private readonly object _shownLock = new object();
        private Form _uiAbout;
        private bool _isDisposed;


        public void Dispose()
        {
            lock (_shownLock)
            {
                _uiAbout?.Dispose();
                _uiAbout = null;
                _isDisposed = true;
            }
        }


        public bool IsShown
        {
            get
            {
                lock (_shownLock)
                {
                    return _uiAbout != null;
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
                        _uiAbout.WindowState = FormWindowState.Minimized;
                        _uiAbout.WindowState = FormWindowState.Normal;
                        _uiAbout.Activate();
                    }
                    else
                    {
                        _uiAbout = new AboutForm();
                        _uiAbout.FormClosed += OnAboutClosed;
                        _uiAbout.Show();
                    }
                }
            }
        }


        private void OnAboutClosed(object sender, EventArgs e)
        {
            lock (_shownLock)
            {
                _uiAbout?.Dispose();
                _uiAbout = null;
            }
        }

    }

}
