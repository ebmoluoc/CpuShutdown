using System;

namespace CpuShutdown.UI.Tray.Services.UiNotifyIcon
{
    public interface IUiNotifyIcon
    {
        void SetSettingsClickHandler(EventHandler onSettingsClick);
        void SetAboutClickHandler(EventHandler onAboutClick);
        void SetTemperature(int temperature);
        void ShowAlert(int temperature);
        void SetRedIcon();
        void SetYellowIcon();
        void SetGreenIcon();
    }
}
