using System;

namespace CpuShutdown.Services.SensLogon2
{
    public sealed class SensLogon2EventArgs : EventArgs
    {
        public SensLogon2EventArgs(SensLogon2EventType eventType, string userName, uint sessionId)
        {
            EventType = eventType;
            UserName = userName;
            SessionId = sessionId;
        }

        public SensLogon2EventType EventType { get; }
        public string UserName { get; }
        public uint SessionId { get; }
    }
}
