using CpuShutdown.Settings;
using System;

namespace CpuShutdown.Services.ArgsReader
{

    public sealed class ArgsReader : IArgsReader
    {

        public string[] Args { get; set; } = Environment.GetCommandLineArgs();


        public string MutexName => GetArgument(AppSettings.MutexNameSwitch, Args);


        public string PipeHandle => GetArgument(AppSettings.PipeHandleSwitch, Args);


        private static string GetArgument(string prefix, string[] args)
        {
            foreach (var arg in args)
                if (arg.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    return arg.Substring(prefix.Length);

            return null;
        }

    }

}
