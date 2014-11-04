using System;
using System.Collections.Generic;
using System.Text;

namespace Shane.Church.StirlingMoney.Universal
{
    public static class LiveConfig
    {
        public static readonly string ClientId = "00000000400C1EB8";
        public static readonly string[] Scopes = new string[4] { "wl.basic", "wl.offline_access", "wl.skydrive_update", "wl.emails" };
    }
}
