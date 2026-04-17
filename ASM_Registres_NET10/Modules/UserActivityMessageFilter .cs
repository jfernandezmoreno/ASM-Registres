using System;
using System.Windows.Forms;

namespace ASM_Registres_NET10.Modules
{
    public class UserActivityMessageFilter : IMessageFilter
    {
        private readonly Action onUserActivity;

        public UserActivityMessageFilter(Action onUserActivity)
        {
            this.onUserActivity = onUserActivity;
        }

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg >= 0x100 && m.Msg <= 0x109 || m.Msg >= 0x200 && m.Msg <= 0x209)
            {
                onUserActivity?.Invoke(); 
            }
            return false; 
        }
    }
}
