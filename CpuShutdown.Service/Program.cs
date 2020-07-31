using CpuShutdown.Services.ArgsReader;
using CpuShutdown.Services.CpuSensor;
using CpuShutdown.Services.Ipc;
using CpuShutdown.Services.SensLogon2;
using CpuShutdown.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace CpuShutdown.Service
{

    public sealed class Program
    {

        public static void Main()
        {
            TaskScheduler.UnobservedTaskException += OnTaskSchedulerUnobservedTaskException;
            AppDomain.CurrentDomain.UnhandledException += OnAppDomainUnhandledException;

            var serviceMutexName = new ArgsReader().MutexName;
            if (serviceMutexName == AppSettings.ServiceMutexName)
            {
                AppSettings.Initialize();
                Log.Logger = AppSettings.Logger;

                using var serviceMutex = Helpers.CreateOwnedMutex($"Global\\{AppSettings.ServiceMutexName}");

                var host = CreateHost();
                host.Run();

                serviceMutex.ReleaseMutex();

                Log.CloseAndFlush();
            }
        }


        private static IHost CreateHost()
        {
            var host = Host.CreateDefaultBuilder();

            host.ConfigureServices((hostContext, services) =>
            {
                services.AddOptions<ServiceSettings>().Bind(hostContext.Configuration.GetSection(nameof(ServiceSettings))).ValidateDataAnnotations();
                services.AddTransient<ISensLogon2, SensLogon2>();
                services.AddTransient<IIpcServer, IpcServer>();
                services.AddTransient<ICpuSensor, CpuSensor>();
                services.AddHostedService<Worker>();
            });

            host.UseSerilog();
            host.UseWindowsService();

            return host.Build();
        }


        private static void ExceptionLogger(Exception ex, [CallerMemberName] string callerName = "")
        {
            Log.Fatal(ex, $"Unhandled Exception ({typeof(Program).Assembly.GetName().Name}) caught by {callerName}");
            Log.CloseAndFlush();
            Environment.Exit(1);
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

    }

}
