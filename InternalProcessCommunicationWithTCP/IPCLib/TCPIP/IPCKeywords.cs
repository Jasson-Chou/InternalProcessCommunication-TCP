using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCLib.TCPIP
{
    internal static class IPCKeywords
    {
        internal static int BufferSize = 1024;

        internal static readonly string AskID = "ID?";

        internal static readonly string[] Keywords = new string[] { AskID };

        internal static bool IsKeyword(string value) => Keywords.Contains(value);

        internal static string PortNumberFileName(string name) => Path.Combine(Path.GetTempPath(), "de502836-ba39-4c66-97f3-521e76928c5d",
            name + ".bin");

        internal static string ACEKey = "8d24cc2f74f84003";
    }
}
 