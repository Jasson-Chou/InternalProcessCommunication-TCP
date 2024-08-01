using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCLib.TCPIP
{
    public static class IPCKeywords
    {
        public static int BufferSize = 1024;

        public static readonly string AskID = "ID?";

        public static readonly string[] Keywords = new string[] { AskID };

        public static bool IsKeyword(string value) => Keywords.Contains(value);
    }
}
 