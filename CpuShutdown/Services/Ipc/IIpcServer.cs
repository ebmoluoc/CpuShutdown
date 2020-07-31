namespace CpuShutdown.Services.Ipc
{
    public interface IIpcServer
    {
        bool IsOpen { get; }
        void Open(uint sessionId);
        bool Close();
        void SendData(int temperature, IpcCommand command);
    }
}
