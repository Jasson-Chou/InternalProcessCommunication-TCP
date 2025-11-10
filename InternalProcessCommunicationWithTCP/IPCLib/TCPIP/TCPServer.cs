using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        public TCPServer(string name)
        {
            Clients = new List<TCPClient>();
            this.serverName = name;
        }

        public bool Created { get; private set; }
        public int Port { get; private set; }
        public List<TCPClient> Clients { get; }

        private readonly string serverName;

        public async Task StartListener()
        {
            try
            {
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                Port = await FindAvailablePortAsync();
                var tempPath = IPCKeywords.PortNumberFileName(serverName);
                var aceValue = AesEncryption.EncryptString(Port.ToString(), IPCKeywords.ACEKey);
                var decryValue = AesEncryption.DecryptString(aceValue, IPCKeywords.ACEKey);
                var fileFolder = Path.GetDirectoryName(tempPath);
                if (!Directory.Exists(fileFolder))
                {
                    Directory.CreateDirectory(fileFolder);
                }

                File.WriteAllText(tempPath, aceValue);

                server = new TcpListener(localAddr, Port);
                server.Start();

                Trace.WriteLine($"Create Listener Port: {Port}");

                Created = true;

                

                while (true)
                {
                    var client = await server.AcceptTcpClientAsync();

                    var tcpClient = new TCPClient(client);

                    var tryGetID = tcpClient.Read(100);

                    if(string.IsNullOrEmpty(tryGetID) || tryGetID.Length <= IPCKeywords.AskID.Length || !tryGetID.Contains(IPCKeywords.AskID))
                    {
                        tcpClient.Disconnect();
                        continue;
                    }

                    await tcpClient.WriteAsync("OK");

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
                Created = false;
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
            for (int port = MaxPort; port >= MinPort; port--)
            {
                if (await IsPortAvailableAsync(port))
                {
                    return port;
                }
            }

            throw new Exception("Not Found And Port Number");
        }

        static async Task<bool> IsPortAvailableAsync(int port)
        {
            try
            {
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
