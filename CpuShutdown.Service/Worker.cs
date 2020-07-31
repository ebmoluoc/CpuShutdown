using CpuShutdown.Services.CpuSensor;
using CpuShutdown.Services.Ipc;
using CpuShutdown.Services.SensLogon2;
using CpuShutdown.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CpuShutdown.Service
{

    public sealed class Worker : BackgroundService
    {

        private readonly IOptions<ServiceSettings> _options;
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly IIpcServer _ipcServer;
        private readonly ICpuSensor _cpuSensor;


        public Worker(IOptions<ServiceSettings> options, IHostApplicationLifetime applicationLifetime, ISensLogon2 sensLogon2, IIpcServer ipcServer, ICpuSensor cpuSensor)
        {
            _options = options;
            _ipcServer = ipcServer;
            _cpuSensor = cpuSensor;

            _applicationLifetime = applicationLifetime;
            _applicationLifetime.ApplicationStarted.Register(OnApplicationStarted);
            _applicationLifetime.ApplicationStopped.Register(OnApplicationStopped);

            sensLogon2.SensLogon2Event += OnSensLogon2Event;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var pollingInterval = _options.Value.PollingInterval * 1000;
            var yellowTemperature = _options.Value.YellowTemperature;
            var redTemperature = _options.Value.RedTemperature;
            var alertTemperature = _options.Value.AlertTemperature;
            var alertHysteresis = _options.Value.AlertHysteresis;
            var shutdownTemperature = _options.Value.ShutdownTemperature;
            var shutdownTimeout = _options.Value.ShutdownTimeout;
            var previousTemp = int.MaxValue;
            var alertFlag = false;

            while (!stoppingToken.IsCancellationRequested)
            {
                var command = IpcCommand.Green;
                var currentTemp = _cpuSensor.ReadTemperature();

                if (currentTemp != previousTemp)
                {
                    previousTemp = currentTemp;

                    if (currentTemp >= shutdownTemperature)
                    {
                        command = IpcCommand.Close;

                        Log.Fatal($"System shutdown : {currentTemp}°C");
                        Helpers.SystemShutdown($"System shutdown initiated by {AppSettings.ApplicationName} : {currentTemp}°C", _ipcServer.IsOpen ? shutdownTimeout : 0);

                        _applicationLifetime.StopApplication();
                    }
                    else if (currentTemp >= alertTemperature && !alertFlag)
                    {
                        command = IpcCommand.Alert;
                        alertFlag = true;

                        Log.Warning($"CPU temperature alert : {currentTemp}°C");
                    }
                    else
                    {
                        if (currentTemp >= redTemperature)
                            command = IpcCommand.Red;
                        else if (currentTemp >= yellowTemperature)
                            command = IpcCommand.Yellow;

                        if (alertFlag && alertTemperature - currentTemp >= alertHysteresis)
                            alertFlag = false;
                    }

                    if (_ipcServer.IsOpen)
                        SendIpcServer(currentTemp, command);
                }

                await Task.Delay(pollingInterval, stoppingToken);
            }
        }


        private void OnApplicationStarted()
        {
            Log.Information("Service Started");

            var sessionId = Helpers.GetActiveSessionId();

            if (Helpers.IsUserLogged(sessionId))
                OpenIpcServer(sessionId, "ServiceStarted");
        }


        private void OnApplicationStopped()
        {
            Log.Information("Service Stopped");

            CloseIpcServer("ServiceStopped");
        }


        private void OnSensLogon2Event(object sender, SensLogon2EventArgs e)
        {
            switch (e.EventType)
            {
                case SensLogon2EventType.Logon:
                    OpenIpcServer(e.SessionId, nameof(SensLogon2EventType.Logon));
                    break;
                case SensLogon2EventType.SessionReconnect:
                    OpenIpcServer(e.SessionId, nameof(SensLogon2EventType.SessionReconnect));
                    break;
                case SensLogon2EventType.Logoff:
                    CloseIpcServer(nameof(SensLogon2EventType.Logoff));
                    break;
                case SensLogon2EventType.SessionDisconnect:
                    CloseIpcServer(nameof(SensLogon2EventType.SessionDisconnect));
                    break;
            }
        }


        private void OpenIpcServer(uint sessionId, string eventName)
        {
            try
            {
                _ipcServer.Open(sessionId);
                Log.Information($"IPC server opened by {eventName}");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "IPC server opening");
            }
        }


        private void CloseIpcServer(string eventName)
        {
            if (_ipcServer.Close())
                Log.Information($"IPC server closed by {eventName}");
        }


        private void SendIpcServer(int temperature, IpcCommand command)
        {
            try
            {
                _ipcServer.SendData(temperature, command);
            }
            catch (Exception ex)
            {
                string eventName;

                if (ex is IOException)
                {
                    eventName = "IpcClientClosed";
                }
                else if (ex is NullReferenceException || ex is ObjectDisposedException)
                {
                    eventName = "NullOrDisposedPipe";
                }
                else
                {
                    eventName = ex.GetType().Name;
                    Log.Error(ex, "IPC server sending");
                }

                CloseIpcServer(eventName);
            }
        }

    }

}
