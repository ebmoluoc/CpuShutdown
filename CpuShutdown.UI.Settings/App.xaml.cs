using CpuShutdown.Settings;
using Serilog;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace CpuShutdown.UI.Settings
{

    public partial class App : Application
    {

        private Mutex _uiSettingsMutex;


        private void Application_Startup(object sender, StartupEventArgs e)
        {
            TaskScheduler.UnobservedTaskException += OnTaskSchedulerUnobservedTaskException;
            AppDomain.CurrentDomain.UnhandledException += OnAppDomainUnhandledException;
            Current.Dispatcher.UnhandledException += OnApplicationUnhandledException;

            EventManager.RegisterClassHandler(typeof(TextBox), UIElement.PreviewMouseLeftButtonDownEvent, new RoutedEventHandler(OnTextBoxPreviewMouseLeftButtonDown), true);
            EventManager.RegisterClassHandler(typeof(TextBox), UIElement.GotKeyboardFocusEvent, new RoutedEventHandler(OnTextBoxGotKeyboardFocus), true);

            Log.Logger = AppSettings.Logger;

            try
            {
                if (!Helpers.IsServiceRunning(AppSettings.ServiceName))
                    throw new InvalidOperationException("Service not running");

                _uiSettingsMutex = Helpers.CreateOwnedMutex("902B4B8F-F880-4B40-8EBC-61566A9D8348");
            }
            catch (InvalidOperationException)
            {
                Shutdown();
            }

            MainWindow = new SettingsView();
            MainWindow.Show();
        }


        private void Application_Exit(object sender, ExitEventArgs e)
        {
            _uiSettingsMutex?.ReleaseMutex();
            _uiSettingsMutex?.Dispose();

            Log.CloseAndFlush();
        }


        private static void OnTextBoxPreviewMouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && !textBox.IsKeyboardFocusWithin)
            {
                textBox.Focus();
                e.Handled = true;
            }
        }


        private static void OnTextBoxGotKeyboardFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
                textBox.SelectAll();
        }


        private static void ExceptionLogger(Exception ex, [CallerMemberName] string callerName = "")
        {
            Log.Fatal(ex, $"Unhandled Exception ({typeof(App).Assembly.GetName().Name}) caught by {callerName}");
            MessageBox.Show($"Settings editor application has encountered an unexpected error and needs to close.\n\n{ex?.Message}", AppSettings.ApplicationName, MessageBoxButton.OK, MessageBoxImage.Error);
            Current.Dispatcher.BeginInvoke((Action)Current.Shutdown);
        }


        private static void OnTaskSchedulerUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();
            ExceptionLogger(e.Exception);
        }


        private static void OnAppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ExceptionLogger(e.ExceptionObject as Exception);
        }


        private static void OnApplicationUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            ExceptionLogger(e.Exception);
        }

    }

}
