using CpuShutdown.Interops;
using Serilog;
using Serilog.Events;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text.Json;

namespace CpuShutdown.Settings
{

    public sealed class AppSettings : SettingsBase
    {

        private ServiceSettings _serviceSettings;
        [Required(ErrorMessage = "{0} is required")]
        public ServiceSettings ServiceSettings
        {
            get { return _serviceSettings; }
            set { SetProperty(ref _serviceSettings, value); }
        }

        public void Save()
        {
            if (HasErrors)
                throw new InvalidOperationException("Cannot save settings containing errors");

            var json = JsonSerializer.Serialize(this);

            File.WriteAllText(AppSettingsJsonPath, json);
        }


        internal static string AppDirectory => Path.GetDirectoryName(typeof(AppSettings).Assembly.Location);

        internal static string AppProgramData => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), ApplicationName);

        internal static string LogFilePath => Path.Combine(AppProgramData, "CpuShutdown.log");

        internal static string AppSettingsJsonPath => Path.Combine(AppDirectory, "appsettings.json");

        internal static string UiSettingsPath => Path.Combine(AppDirectory, "CpuShutdown.UI.Settings.exe");

        internal static string UiTrayPath => Path.Combine(AppDirectory, "CpuShutdown.UI.Tray.exe");

        internal static string PipeHandleSwitch => "-p:";

        public static string ApplicationName => "CPU Shutdown";

        public static Version ApplicationVersion => typeof(AppSettings).Assembly.GetName().Version;

        public static string ApplicationLicense => Helpers.StringFromEmbeddedResource(typeof(AppSettings).Assembly, "LICENSE");

        public static string ServiceName => "CpuShutdownSvc";

        public static ILogger Logger => new LoggerConfiguration().MinimumLevel.Override("Microsoft", LogEventLevel.Warning).WriteTo.File(LogFilePath, fileSizeLimitBytes: 10485760, rollingInterval: RollingInterval.Month, retainedFileCountLimit: 2, shared: true).CreateLogger();

        public static int WM_ACTIVATE_UI_SETTINGS => NativeMethods.RegisterWindowMessage("906CBE76-6083-4C4C-912E-5F576FD26648");

        public static AppSettings Load()
        {
            var text = File.ReadAllText(AppSettingsJsonPath);
            var appSettings = JsonSerializer.Deserialize<AppSettings>(text);

            if (appSettings.HasErrors)
                throw new InvalidOperationException("Cannot load settings containing errors");

            return appSettings;
        }

        /// <summary>
        /// Because there is one log file shared by all processes and not all processes have admin privilege,
        /// we need to make sure the folder containing the log file have write access for the users group.
        /// This method is the first thing the Windows Service should call because it is the entry point
        /// of this application.
        /// </summary>
        public static void Initialize()
        {
            try
            {
                var directoryInfo = new DirectoryInfo(AppProgramData);
                directoryInfo.Create();

                var identityReference = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);
                var accessRule = new FileSystemAccessRule(identityReference, FileSystemRights.Write, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow);

                var directorySecurity = directoryInfo.GetAccessControl(AccessControlSections.Access);
                directorySecurity.AddAccessRule(accessRule);

                directoryInfo.SetAccessControl(directorySecurity);
            }
            catch (Exception)
            {
                // Swallow the exception to make sure the Windows Service will keep running. As
                // long as the system shuts down when the CPU overheats, logging is a lesser problem.
            }
        }

    }

}
