using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IPCLib.TCPIP
{
    public class TCPServer
    {
        private TcpListener server = null;

        public event Action<TcpClient> OnClientConnect = null;

        public TCPServer(int port)
        {
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");
            this.Port = port;
            server = new TcpListener(localAddr, Port);
            Clients = new List<TcpClient>();
        }

        public int Port { get; }
        public List<TcpClient> Clients { get; }

        public async Task WaitClientConnection()
        {
            while (true)
            {
                TcpClient client = await server.AcceptTcpClientAsync();
                Clients.Add(client);
                OnClientConnect?.Invoke(client);
            }
        }


    }
}
