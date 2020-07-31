using CpuShutdown.Settings;
using System;
using System.Media;
using System.Windows.Forms;

namespace CpuShutdown.UI.Tray.Services.UiNotifyIcon
{

    public sealed class UiNotifyIcon : IUiNotifyIcon, IDisposable
    {

        private readonly NotifyIcon _notifyIcon = new NotifyIcon { Text = AppSettings.ApplicationName, Visible = true };
        private readonly ContextMenuStrip _contextMenuStrip = new ContextMenuStrip { ShowImageMargin = false };
        private readonly SoundPlayer _soundPlayer = new SoundPlayer(Properties.Resources.AudioAlert);


        public UiNotifyIcon()
        {
            _notifyIcon.ContextMenuStrip = _contextMenuStrip;
        }


        public void Dispose()
        {
            _soundPlayer.Dispose();
            _contextMenuStrip.Dispose();
            _notifyIcon.Dispose();
        }


        public void SetSettingsClickHandler(EventHandler onSettingsClick)
        {
            _contextMenuStrip.Items.Add("Settings", null, onSettingsClick);
        }


        public void SetAboutClickHandler(EventHandler onAboutClick)
        {
            _contextMenuStrip.Items.Add($"About {AppSettings.ApplicationName}", null, onAboutClick);
        }


        public void SetTemperature(int temperature)
        {
            _notifyIcon.Text = $"CPU Temperature : {temperature}°C";
        }


        public void ShowAlert(int temperature)
        {
            _notifyIcon.Icon = Properties.Resources.IconRed;
            _notifyIcon.ShowBalloonTip(5000, AppSettings.ApplicationName, $"CPU Temperature Alert : {temperature}°C", ToolTipIcon.None);
            _soundPlayer.Play();
        }


        public void SetRedIcon()
        {
            _notifyIcon.Icon = Properties.Resources.IconRed;
        }


        public void SetYellowIcon()
        {
            _notifyIcon.Icon = Properties.Resources.IconYellow;
        }


        public void SetGreenIcon()
        {
            _notifyIcon.Icon = Properties.Resources.IconGreen;
        }

    }

}
