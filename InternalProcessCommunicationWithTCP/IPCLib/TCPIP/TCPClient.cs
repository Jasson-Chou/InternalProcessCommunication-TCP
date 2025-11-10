using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IPCLib.TCPIP
{

    public delegate void OnReceiveHandler(object sender, string message);
    public class TCPClient
    {
        private readonly bool clientInstance;
        private CancellationTokenSource readCancelTokenSrc;
        internal TcpClient client;
        internal NetworkStream stream;
        public bool IsReading => readCancelTokenSrc != null;
        public event OnReceiveHandler OnReceive = null;

        private const int MinPort = 49152;
        private const int MaxPort = 65535;
        public string ID { get; internal set; }

        private readonly string serverName;
        internal TCPClient(TcpClient client)
        {
            this.client = client;
            stream = client.GetStream();
            clientInstance = false;
        }

        public TCPClient(string ID, string ServerName)
        {
            this.client = new TcpClient();
            clientInstance = true;
            this.ID = ID;
            this.serverName = ServerName;
        }

        public async Task<bool> ConnectAsync()
        {
            try
            {
                if (!clientInstance) throw new Exception($"Server Can be not Connect Client!");

                try
                {
                    var tempPath = IPCKeywords.PortNumberFileName(serverName);
                    
                    if (!File.Exists(tempPath)) return false;

                    var portStr = File.ReadAllText(tempPath);
                    var decryValue = AesEncryption.DecryptString(portStr, IPCKeywords.ACEKey);
                    var parseResult = int.TryParse(decryValue, out int port);

                    if (!parseResult) return false;

                    await client.ConnectAsync("127.0.0.1", port);
                    stream = client.GetStream();
                    await WriteAsync(IPCKeywords.AskID + ID);
                    var assert = Read(200);

                    if (!string.Equals(assert, "OK"))
                    {
                        Disconnect();
                        return false;
                    }
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.ToString());
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
            }

            return false;
        }

        public bool Disconnect()
        {
            try
            {
                // 關閉流和客戶端
                if(client.Connected)
                {
                    stream.Close();
                    client.Close();
                    if (clientInstance)
                        client = new TcpClient();
                    Trace.WriteLine("Disconnected!");
                }
                return true;
            }
            catch (Exception e)
            {
                Trace.WriteLine($"Disconnect Error: {e.Message}");
                return false;
            }
        }

        public bool Write(string msg)
        {
            try
            {
                byte[] data = Encoding.ASCII.GetBytes(msg);
                stream.Write(data, 0, data.Length);
                return true;
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
                return false;
            }
        }

        public async Task<bool> WriteAsync(string msg)
        {

            try
            {
                byte[] data = Encoding.ASCII.GetBytes(msg);
                await stream.WriteAsync(data, 0, data.Length);
                return true;
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
                return false;
            }
        }

        public string Read(int timeout = -1)
        {
            byte[] buffer = new byte[IPCKeywords.BufferSize];
            int bytesRead;
            
            try
            {
                stream.ReadTimeout = timeout;

                bytesRead = stream.Read(buffer, 0, buffer.Length);

                stream.ReadTimeout = -1;

                string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                
                return data;
            }
            catch(Exception e)
            {
                Trace.WriteLine(e.ToString());
            }

            return null;
        }
        


        public async void BeginRead()
        {
            byte[] buffer = new byte[IPCKeywords.BufferSize];
            int bytesRead;

            try
            {
                readCancelTokenSrc = new CancellationTokenSource();
                while (client.Connected)
                {
                    bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, readCancelTokenSrc.Token);

                    if(bytesRead == 0)
                    {
                        Trace.WriteLine($"client[{ID}] Disconnected.");
                        break;
                    }

                    string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    
                    RaiseOnReceive(data);
                }
            }
            catch(OperationCanceledException e)
            {
                Trace.WriteLine(e.ToString());
            }
            catch(Exception e)
            {
                Trace.WriteLine(e.ToString());
            }
            finally
            {
                readCancelTokenSrc?.Dispose();
                readCancelTokenSrc = null;
            }
        }

        public void EndRead()
        {
            readCancelTokenSrc?.Cancel();
        }


        internal void RaiseOnReceive(string message)
        {
            OnReceive?.Invoke(this, message);
        }




    }
}
