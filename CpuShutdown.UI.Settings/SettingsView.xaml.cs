using CpuShutdown.Settings;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;

namespace CpuShutdown.UI.Settings
{

    public partial class SettingsView : Window
    {

        private static readonly int _ActivateMessage = AppSettings.WM_ACTIVATE_UI_SETTINGS;


        public SettingsView()
        {
            InitializeComponent();

            var appSettings = AppSettings.Load();
            appSettings.ServiceSettings.PropertyChanged += (object sender, PropertyChangedEventArgs e) => buttonSave.IsEnabled = !((ServiceSettings)sender).HasErrors;

            DataContext = appSettings;

            Title = AppSettings.ApplicationName;
        }


        protected override void OnSourceInitialized(EventArgs e)
        {
            var windowHandle = new WindowInteropHelper(this).Handle;

            Helpers.RemoveSystemMenuMaximize(windowHandle);
            Helpers.RemoveSystemMenuSize(windowHandle);

            Helpers.AllowWindowMessage(windowHandle, _ActivateMessage);

            if (PresentationSource.FromVisual(this) is HwndSource hwndSource)
                hwndSource.AddHook(WndProc);

            base.OnSourceInitialized(e);
        }


        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == _ActivateMessage)
            {
                WindowState = WindowState.Minimized;
                WindowState = WindowState.Normal;
                Activate();

                handled = true;
            }

            return IntPtr.Zero;
        }


        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var appSettings = (AppSettings)DataContext;
            appSettings.Save();

            if (Helpers.IsServiceRunning(AppSettings.ServiceName))
                Helpers.RestartService(AppSettings.ServiceName);

            Close();
        }


        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }

}
