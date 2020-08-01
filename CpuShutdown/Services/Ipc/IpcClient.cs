using CpuShutdown.Interops;
using System;
using System.Buffers.Binary;
using System.IO.Pipes;
using System.Threading;

namespace CpuShutdown.Services.Ipc
{

    public sealed class IpcClient : IIpcClient, IDisposable
    {

        private readonly object _openLock = new object();
        private readonly byte[] _bufferData = new byte[8];
        private AnonymousPipeClientStream _pipeData;
        private Thread _threadData;
        private volatile bool _threadCancel;


        public event EventHandler<IpcClientDataEventArgs> IpcClientDataEvent;


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
                    return _threadData?.IsAlive == true;
                }
            }
        }


        public void Open(string pipeDataHandle)
        {
            lock (_openLock)
            {
                if (IsOpen)
                    throw new InvalidOperationException("IPC client already open");

                _pipeData = new AnonymousPipeClientStream(PipeDirection.In, pipeDataHandle);

                try
                {
                    _threadData = new Thread(DataReader);
                    _threadData.Start();
                }
                catch (Exception)
                {
                    _pipeData.Dispose();
                    _pipeData = null;

                    _threadData = null;

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
                    _threadCancel = true;
                    NativeMethods.CancelIoEx(_pipeData.SafePipeHandle, IntPtr.Zero);

                    _pipeData.Dispose();
                    _pipeData = null;

                    _threadData = null;
                }

                return isOpen;
            }
        }


        private void DataReader()
        {
            var buffer = _bufferData.AsSpan();

            while (!_threadCancel)
            {
                int bytesCount;

                try
                {
                    bytesCount = _pipeData.Read(buffer);
                }
                catch (OperationCanceledException)
                {
                    return;
                }

                if (bytesCount == buffer.Length)
                {
                    var temperature = BinaryPrimitives.ReadInt32LittleEndian(buffer.Slice(0));
                    var command = (IpcCommand)BinaryPrimitives.ReadInt32LittleEndian(buffer.Slice(4));

                    IpcClientDataEvent?.Invoke(this, new IpcClientDataEventArgs(temperature, command));
                }
                else if (bytesCount == 0)
                {
                    throw new InvalidOperationException($"Communication with IPC server broken");
                }
                else
                {
                    throw new InvalidOperationException($"Number of bytes read by IPC client was unexpected ({bytesCount} bytes instead of {buffer.Length})");
                }
            }
        }

    }

}
