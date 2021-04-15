using CpuShutdown.Services.ArgsReader;
using CpuShutdown.Services.Ipc;
using CpuShutdown.UI.Tray.Services.UiDialogs;
using CpuShutdown.UI.Tray.Services.UiNotifyIcon;
using System;
using System.Windows.Forms;

namespace CpuShutdown.UI.Tray
{

    public sealed class SystemTray : ApplicationContext
    {

        private readonly IIpcClient _ipcClient;
        private readonly IUiNotifyIcon _uiNotifyIcon;


        public SystemTray(IArgsReader argsReader, IIpcClient ipcClient, IUiNotifyIcon uiNotifyIcon, IUiDialog uiSettings, IUiDialog uiAbout)
        {
            _uiNotifyIcon = uiNotifyIcon;
            _uiNotifyIcon.SetSettingsClickHandler((object sender, EventArgs e) => uiSettings.Show());
            _uiNotifyIcon.SetAboutClickHandler((object sender, EventArgs e) => uiAbout.Show());

            _ipcClient = ipcClient;
            _ipcClient.IpcClientDataEvent += OnDataEvent;
            _ipcClient.Open(argsReader.PipeHandle);
        }


        private void OnDataEvent(object sender, IpcClientDataEventArgs e)
        {
            _uiNotifyIcon.SetTemperature(e.Temperature);

            switch (e.Command)
            {
                case IpcCommand.Close:
                    _ipcClient.Close();
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
