using CpuShutdown.Services.ArgsReader;
using CpuShutdown.Services.Ipc;
using CpuShutdown.Services.UiSettings;
using CpuShutdown.Settings;
using CpuShutdown.UI.Tray.Services.UiAbout;
using CpuShutdown.UI.Tray.Services.UiNotifyIcon;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CpuShutdown.UI.Tray
{

    public static class Program
    {

        [STAThread]
        public static void Main()
        {
            TaskScheduler.UnobservedTaskException += OnTaskSchedulerUnobservedTaskException;
            AppDomain.CurrentDomain.UnhandledException += OnAppDomainUnhandledException;
            Application.ThreadException += OnApplicationThreadException;

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var uiTrayMutexName = new ArgsReader().MutexName;
            if (uiTrayMutexName == AppSettings.UiTrayMutexName)
            {
                Log.Logger = AppSettings.Logger;

                using var uiTrayMutex = Helpers.CreateOwnedMutex(uiTrayMutexName);

                using var serviceProvider = CreateServiceProvider();
                var systemTray = serviceProvider.GetRequiredService<SystemTray>();
                Application.Run(systemTray);

                uiTrayMutex.ReleaseMutex();

                Log.CloseAndFlush();
            }
        }


        private static ServiceProvider CreateServiceProvider()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddTransient<IArgsReader, ArgsReader>();
            serviceCollection.AddTransient<IIpcClient, IpcClient>();
            serviceCollection.AddTransient<IUiNotifyIcon, UiNotifyIcon>();
            serviceCollection.AddTransient<IUiSettings, UiSettings>();
            serviceCollection.AddTransient<IUiAbout, UiAbout>();
            serviceCollection.AddTransient<SystemTray>();

            return serviceCollection.BuildServiceProvider();
        }


        private static void ExceptionLogger(Exception ex, [CallerMemberName] string callerName = "")
        {
            Log.Fatal(ex, $"Unhandled Exception ({typeof(Program).Assembly.GetName().Name}) caught by {callerName}");
            Log.CloseAndFlush();
            MessageBox.Show($"System tray application has encountered an unexpected error and needs to close.\n\n{ex?.Message}", AppSettings.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            Application.Exit();
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


        private static void OnApplicationThreadException(object sender, ThreadExceptionEventArgs e)
        {
            ExceptionLogger(e.Exception);
        }

    }

}
