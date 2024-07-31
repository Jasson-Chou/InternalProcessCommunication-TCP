using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IPCLib.TCPIP
{
    public class TCPClient
    {
        private readonly bool clientInstance;
        private TcpClient client;
        private NetworkStream stream;

        internal TCPClient(TcpClient client)
        {
            this.client = client;
            clientInstance = false;
        }

        public TCPClient(int port)
        {
            this.client = new TcpClient();
            clientInstance = true;
        }

        public async Task<bool> ConnectAsync(int port)
        {
            try
            {
                await client.ConnectAsync("127.0.0.1", port);
                stream = client.GetStream();
                return true;
            }
            catch(Exception e)
            {
                Trace.WriteLine(e.ToString());
                return false;
            }
        }

        public async Task<bool> DisconnectAsync()
        {
            try
            {
                // 發送斷開連接的消息
                string disconnectMessage = "DISCONNECT";
                byte[] data = Encoding.ASCII.GetBytes(disconnectMessage);
                await stream.WriteAsync(data, 0, data.Length);
                Console.WriteLine("已發送斷開連接請求");

                // 關閉流和客戶端
                stream.Close();
                client.Close();
                Console.WriteLine("已斷開與服務器的連接");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"斷開連接錯誤: {e.Message}");
                return false;
            }
        }

    }
}
