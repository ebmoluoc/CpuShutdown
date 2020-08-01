﻿using CpuShutdown.Services.ArgsReader;
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

            var uiSettingsGuid = new ArgsReader().ProjectGuid;
            if (uiSettingsGuid != AppSettings.UiSettingsProjectGuid)
            {
                Shutdown();
            }
            else
            {
                Log.Logger = AppSettings.Logger;

                _uiSettingsMutex = Helpers.CreateOwnedMutex(uiSettingsGuid);

                MainWindow = new SettingsView();
                MainWindow.Show();
            }
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
