using CpuShutdown.Settings;
using System;
using System.Buffers.Binary;
using System.IO;
using System.IO.Pipes;

namespace CpuShutdown.Services.Ipc
{

    public sealed class IpcServer : IIpcServer, IDisposable
    {

        private readonly object _openLock = new object();
        private readonly byte[] _bufferData = new byte[8];
        private AnonymousPipeServerStream _pipeData;
        private uint _clientProcessId;


        public void Dispose()
        {
            Close();
        }


        public bool IsOpen
        {
            get
            {
                lock (_openLock)
                {
                    return _clientProcessId != 0;
                }
            }
        }


        public void Open(uint sessionId)
        {
            lock (_openLock)
            {
                if (IsOpen)
                    throw new InvalidOperationException("IPC server already open");

                _pipeData = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable);

                try
                {
                    var commandLine = $"\"{AppSettings.UiTrayPath}\" {AppSettings.PipeHandleSwitch}{_pipeData.GetClientHandleAsString()}";
                    _clientProcessId = Helpers.CreateProcessAsUser(null, commandLine, sessionId);

                    _pipeData.DisposeLocalCopyOfClientHandle();
                }
                catch (Exception)
                {
                    _pipeData.Dispose();
                    _pipeData = null;

                    _clientProcessId = 0;

                    throw;
                }
            }
        }


        public bool Close()
        {
            lock (_openLock)
            {
                var isOpen = IsOpen;

                if (isOpen)
                {
                    try
                    {
                        SendData(0, IpcCommand.Close);
                    }
                    catch (Exception)
                    {
                        Helpers.TerminateProcess(_clientProcessId);
                    }
                    finally
                    {
                        _pipeData.Dispose();
                        _pipeData = null;

                        _clientProcessId = 0;
                    }
                }

                return isOpen;
            }
        }


        public void SendData(int temperature, IpcCommand command)
        {
            var buffer = _bufferData.AsSpan();
            BinaryPrimitives.WriteInt32LittleEndian(buffer.Slice(0), temperature);
            BinaryPrimitives.WriteInt32LittleEndian(buffer.Slice(4), (int)command);

            _pipeData.Write(buffer);
        }

    }

}
