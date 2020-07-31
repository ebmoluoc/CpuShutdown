using System;

namespace CpuShutdown.Services.Ipc
{
    public interface IIpcClient
    {
        bool IsOpen { get; }
        void Open(string pipeDataHandle);
        bool Close();
        event EventHandler<IpcClientDataEventArgs> IpcClientDataEvent;
    }
}
