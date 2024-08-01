using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        internal TCPClient(TcpClient client)
        {
            this.client = client;
            stream = client.GetStream();
            clientInstance = false;
        }

        public TCPClient(string ID)
        {
            this.client = new TcpClient();
            clientInstance = true;
            this.ID = ID;
        }

        public async Task<bool> ConnectAsync()
        {
            try
            {
                if (!clientInstance) throw new Exception($"Server Can be not Connect Client!");

                for(int port = MaxPort; port >= MinPort; port--)
                {
                    try
                    {
                        await client.ConnectAsync("127.0.0.1", port);
                        stream = client.GetStream();

                        await WriteAsync("127hostAssert");

                        var assert = Read(10);

                        if (string.Equals(assert, "AssertOK"))
                        {
                            Disconnect();
                            continue;
                        }
                    }
                    catch(Exception e)
                    {
                        Trace.WriteLine(e.ToString());
                        continue;
                    }

                    var cmd = Read();

                    if (cmd == IPCKeywords.AskID)
                    {
                        await WriteAsync(IPCKeywords.AskID + ID);
                    }

                    return true;
                }
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
                stream.Close();
                client.Close();
                Trace.WriteLine("Disconnected!");
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
                while (true)
                {
                    bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, readCancelTokenSrc.Token);

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
