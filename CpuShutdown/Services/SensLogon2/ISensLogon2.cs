using System;
using System.Runtime.InteropServices;

namespace CpuShutdown.Services.SensLogon2
{
    [ComImport, Guid("d597bab4-5b9f-11d1-8dd2-00aa004abd5e")]
    public interface ISensLogon2
    {
        void Logon([In, MarshalAs(UnmanagedType.BStr)] string bstrUserName, [In] uint dwSessionId);
        void Logoff([In, MarshalAs(UnmanagedType.BStr)] string bstrUserName, [In] uint dwSessionId);
        void SessionDisconnect([In, MarshalAs(UnmanagedType.BStr)] string bstrUserName, [In] uint dwSessionId);
        void SessionReconnect([In, MarshalAs(UnmanagedType.BStr)] string bstrUserName, [In] uint dwSessionId);
        void PostShell([In, MarshalAs(UnmanagedType.BStr)] string bstrUserName, [In] uint dwSessionId);
        event EventHandler<SensLogon2EventArgs> SensLogon2Event;
    }
}
