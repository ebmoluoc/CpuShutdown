using CpuShutdown.Services.ArgsReader;
using CpuShutdown.Services.Ipc;
using CpuShutdown.Services.UiSettings;
using CpuShutdown.UI.Tray.Services.UiAbout;
using CpuShutdown.UI.Tray.Services.UiNotifyIcon;
using System;
using System.Windows.Forms;

namespace CpuShutdown.UI.Tray
{

    public sealed class SystemTray : ApplicationContext
    {

        private readonly IIpcClient _ipcClient;
        private readonly IUiNotifyIcon _uiNotifyIcon;


        public SystemTray(IArgsReader argsReader, IIpcClient ipcClient, IUiNotifyIcon uiNotifyIcon, IUiSettings uiSettings, IUiAbout uiAbout)
        {
            _uiNotifyIcon = uiNotifyIcon;
            _uiNotifyIcon.SetSettingsClickHandler((object sender, EventArgs e) => uiSettings.Show());
            _uiNotifyIcon.SetAboutClickHandler((object sender, EventArgs e) => uiAbout.Show());

            _ipcClient = ipcClient;
            _ipcClient.IpcClientDataEvent += OnIpcClientDataEvent;
            _ipcClient.Open(argsReader.PipeHandle);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _ipcClient.Close();

            base.Dispose(disposing);
        }


        private void OnIpcClientDataEvent(object sender, IpcClientDataEventArgs e)
        {
            _uiNotifyIcon.SetTemperature(e.Temperature);

            switch (e.Command)
            {
                case IpcCommand.Close:
                    ExitThread();
                    break;
                case IpcCommand.Alert:
                    _uiNotifyIcon.ShowAlert(e.Temperature);
                    break;
                case IpcCommand.Red:
                    _uiNotifyIcon.SetRedIcon();
                    break;
                case IpcCommand.Yellow:
                    _uiNotifyIcon.SetYellowIcon();
                    break;
                case IpcCommand.Green:
                    _uiNotifyIcon.SetGreenIcon();
                    break;
            }
        }

    }

}
