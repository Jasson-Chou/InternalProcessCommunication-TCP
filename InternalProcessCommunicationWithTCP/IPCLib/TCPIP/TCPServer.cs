using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IPCLib.TCPIP
{
    public class TCPServer
    {
        private TcpListener server = null;
        public event Action<TCPClient> OnClientConnect = null;
        private const int MinPort = 49152;
        private const int MaxPort = 65535;
        public TCPServer()
        {
            Clients = new List<TCPClient>();
        }

        public int Port { get; private set; }
        public List<TCPClient> Clients { get; }



        public async Task StartListener()
        {
            try
            {
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                Port = await FindAvailablePortAsync();
                server = new TcpListener(localAddr, Port);
                server.Start();

                Trace.WriteLine($"Create Listener Port: {Port}");

                while (true)
                {
                    var client = await server.AcceptTcpClientAsync();

                    var tcpClient = new TCPClient(client);

                    var assert = tcpClient.Read(10);

                    if (string.Equals(assert, "127hostAssert"))
                    {
                        await tcpClient.WriteAsync("AssertOK");
                    }
                    else
                    {
                        tcpClient.Disconnect();
                        continue;
                    }


                    await tcpClient.WriteAsync(IPCKeywords.AskID);
                    var tryGetID = tcpClient.Read();
                    var id = tryGetID.Remove(0, 3);
                    tcpClient.ID = id;
                    Trace.WriteLine($"Client: {id} connected!");
                    Clients.Add(tcpClient);
                    OnClientConnect?.Invoke(tcpClient);
                }
            }
            catch (OperationCanceledException e)
            {
                Trace.WriteLine($"Listening was canceled. ({e.ToString()})");
            }
            catch (Exception e)
            {
                Trace.WriteLine($"Listening was Exception. ({e.ToString()})");
            }
            finally
            {

            }
        }

        public void StopListener()
        {
            Trace.WriteLine($"Server Stop Listener.");
            server.Stop();
            Clients.Clear();
            Port = -1;
        }

        static async Task<int> FindAvailablePortAsync()
        {
            // 方法 1: 使用動態端口範圍
            for (int port = MaxPort; port >= MinPort; port--)
            {
                if (await IsPortAvailableAsync(port))
                {
                    return port;
                }
            }

            throw new Exception("沒有可用的端口");
        }

        static async Task<bool> IsPortAvailableAsync(int port)
        {
            try
            {
                // 嘗試創建一個 TcpListener 並啟動它
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                var testListener = new TcpListener(localAddr, port);
                await Task.Run(() => testListener.Start());
                testListener.Stop();
                return true;
                
            }
            catch
            {
                return false;
            }
        }
    }
}
