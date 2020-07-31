using CpuShutdown.Interops;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text.Json;

namespace CpuShutdown.Settings
{

    public sealed partial class AppSettings
    {

        internal static string AppDirectory => Path.GetDirectoryName(typeof(AppSettings).Assembly.Location);

        internal static string AppProgramData => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), ApplicationName);

        internal static string LogFilePath => Path.Combine(AppProgramData, "CpuShutdown.log");

        internal static string AppSettingsJsonPath => Path.Combine(AppDirectory, "appsettings.json");

        internal static string UiSettingsPath => Path.Combine(AppDirectory, "CpuShutdown.UI.Settings.exe");

        internal static string UiTrayPath => Path.Combine(AppDirectory, "CpuShutdown.UI.Tray.exe");

        internal static string MutexNameSwitch => "-m:";

        internal static string PipeHandleSwitch => "-p:";

        public static string ApplicationName => "CPU Shutdown";

        public static Version ApplicationVersion => typeof(AppSettings).Assembly.GetName().Version;

        public static string ApplicationLicense => Helpers.StringFromEmbeddedResource(typeof(AppSettings).Assembly, "LICENSE");

        public static string ServiceName => "CpuShutdownSvc";

        public static string UiSettingsMutexName => "fd68dbe5-6841-4370-b8ba-bae77c95dd2b";

        public static string UiTrayMutexName => "fab90191-7c21-4abb-b398-acca5efca162";

        public static string ServiceMutexName => "fd046fd3-bc44-4012-babb-59e0d2fc1e72";

        public static ILogger Logger => new LoggerConfiguration().MinimumLevel.Override("Microsoft", LogEventLevel.Warning).WriteTo.File(LogFilePath, fileSizeLimitBytes: 1048576, rollingInterval: RollingInterval.Month, retainedFileCountLimit: 2, shared: true).CreateLogger();

        public static int WM_ACTIVATE_UI_SETTINGS => NativeMethods.RegisterWindowMessage(UiSettingsMutexName);


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
