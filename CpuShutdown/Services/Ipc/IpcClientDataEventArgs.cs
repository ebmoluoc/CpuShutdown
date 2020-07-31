using System;

namespace CpuShutdown.Services.Ipc
{
    public sealed class IpcClientDataEventArgs : EventArgs
    {
        public IpcClientDataEventArgs(int temperature, IpcCommand command)
        {
            Temperature = temperature;
            Command = command;
        }

        public int Temperature { get; }
        public IpcCommand Command { get; }
    }
}
