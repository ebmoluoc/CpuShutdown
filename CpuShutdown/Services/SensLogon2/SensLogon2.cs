using COMAdmin;
using System;
using System.Runtime.InteropServices;

namespace CpuShutdown.Services.SensLogon2
{

    public sealed class SensLogon2 : ISensLogon2
    {

        private const string SENSGUID_EVENTCLASS_LOGON2 = "{d5978650-5b9f-11d1-8dd2-00aa004abd5e}";


        public event EventHandler<SensLogon2EventArgs> SensLogon2Event;


        public SensLogon2()
        {
            var catalogClass = new COMAdminCatalogClass();
            var catalogCollection = (ICatalogCollection)catalogClass.GetCollection("TransientSubscriptions");
            var catalogObject = (ICatalogObject)catalogCollection.Add();

            catalogObject.set_Value("EventCLSID", SENSGUID_EVENTCLASS_LOGON2);
            catalogObject.set_Value("SubscriberInterface", this);
            catalogCollection.SaveChanges();
        }


        public void Logon([In, MarshalAs(UnmanagedType.BStr)] string bstrUserName, [In] uint dwSessionId)
        {
            SensLogon2Event?.Invoke(this, new SensLogon2EventArgs(SensLogon2EventType.Logon, bstrUserName, dwSessionId));
        }


        public void Logoff([In, MarshalAs(UnmanagedType.BStr)] string bstrUserName, [In] uint dwSessionId)
        {
            SensLogon2Event?.Invoke(this, new SensLogon2EventArgs(SensLogon2EventType.Logoff, bstrUserName, dwSessionId));
        }


        public void SessionDisconnect([In, MarshalAs(UnmanagedType.BStr)] string bstrUserName, [In] uint dwSessionId)
        {
            SensLogon2Event?.Invoke(this, new SensLogon2EventArgs(SensLogon2EventType.SessionDisconnect, bstrUserName, dwSessionId));
        }


        public void SessionReconnect([In, MarshalAs(UnmanagedType.BStr)] string bstrUserName, [In] uint dwSessionId)
        {
            SensLogon2Event?.Invoke(this, new SensLogon2EventArgs(SensLogon2EventType.SessionReconnect, bstrUserName, dwSessionId));
        }


        public void PostShell([In, MarshalAs(UnmanagedType.BStr)] string bstrUserName, [In] uint dwSessionId)
        {
            SensLogon2Event?.Invoke(this, new SensLogon2EventArgs(SensLogon2EventType.PostShell, bstrUserName, dwSessionId));
        }

    }

}
