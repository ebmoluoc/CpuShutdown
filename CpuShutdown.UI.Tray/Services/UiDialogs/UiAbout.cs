using CpuShutdown.UI.Tray.Forms;
using System;
using System.Windows.Forms;

namespace CpuShutdown.UI.Tray.Services.UiDialogs
{

    public sealed class UiAbout : IUiDialog, IDisposable
    {

        private readonly object _shown = new();
        private Form _form;
        private bool _disposed;


        public void Dispose()
        {
            lock (_shown)
            {
                if (IsShown)
                {
                    _form.Dispose();
                    _form = null;
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
                    return _form != null;
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
                        _form.WindowState = FormWindowState.Minimized;
                        _form.WindowState = FormWindowState.Normal;
                        _form.Activate();
                    }
                    else
                    {
                        _form = new AboutForm();
                        _form.FormClosed += OnClosed;
                        _form.Show();
                    }
                }
            }
        }


        private void OnClosed(object sender, EventArgs e)
        {
            lock (_shown)
            {
                _form?.Dispose();
                _form = null;
            }
        }

    }

}
