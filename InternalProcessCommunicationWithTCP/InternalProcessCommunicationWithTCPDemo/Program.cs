using IPCLib.TCPIP;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalProcessCommunicationWithTCPDemo
{
    class Program
    {
        static TCPServer server;
        static List<TCPClient> myClient = new List<TCPClient>();
        static void Main(string[] args)
        {
            server = new TCPServer();
            StartServer();
            foreach (var num in Enumerable.Range(0, 10))
            {
                StartClient("C-" + num).Wait();
            }
            Console.ReadKey();

            string fileName1 = @"C:\Case1.csv";
            string fileName2 = @"C:\Case2.csv";
            string fileName3 = @"C:\Case3.csv";
            string fileName4 = @"C:\Case4.csv";

            Case1(fileName1);
            Case2(fileName2);
            Case3(fileName3);
            Case4(fileName4);

            Console.ReadKey();
        }

        static void Case1(string fileName)
        {
            var serClient = server.Clients.ElementAt(0);
            var tcpClient = myClient[0];

            StringBuilder sb = new StringBuilder(10 * 1024 * 1024);

            var randomString = GetRandomString(1024);

            serClient.Write(randomString);
            var getStr = tcpClient.Read();

            TimeSpan totalTime = TimeSpan.FromTicks(0);
            for (int i = 0; i < 4096; i++)
            {
                randomString = GetRandomString(1024);

                DateTime startTime = DateTime.Now;

                serClient.Write(randomString);
                var getStr1 = tcpClient.Read();

                TimeSpan watchTime = DateTime.Now - startTime;
                totalTime += watchTime;
                if (getStr1 == randomString)
                {
                    //Console.WriteLine($"[{i:D4}]Correct Spend Time: {watchTime.Ticks / 10.0d} us");
                    LogTimeSpan(sb, i, watchTime);
                }
                else
                {
                    var temp = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[{i:D4}]Incorrect Spend Time: {watchTime.Ticks / 10.0d} us");
                    Console.ForegroundColor = temp;
                }
            }

            Console.WriteLine($"Run Done Spend Time: {totalTime.TotalMilliseconds} ms");

            File.WriteAllText(fileName, sb.ToString());
            File.AppendAllText(fileName, $"Total Spend Time: {totalTime.TotalMilliseconds} ms");
        }

        static void Case2(string fileName)
        {
            var serClient = server.Clients.ElementAt(0);
            var tcpClient = myClient[0];

            StringBuilder sb = new StringBuilder(10 * 1024 * 1024);

            var randomString = GetRandomString(1024);

            serClient.Write(randomString);
            var getStr = tcpClient.Read();

            TimeSpan totalTime = TimeSpan.FromTicks(0);
            for (int i = 0; i < 4096; i++)
            {
                randomString = GetRandomString(1024);

                DateTime startTime = DateTime.Now;

                tcpClient.Write(randomString);
                var getStr1 = serClient.Read();

                TimeSpan watchTime = DateTime.Now - startTime;
                totalTime += watchTime;
                if (getStr1 == randomString)
                {
                    //Console.WriteLine($"[{i:D4}]Correct Spend Time: {watchTime.Ticks / 10.0d} us");
                    LogTimeSpan(sb, i, watchTime);
                }
                else
                {
                    var temp = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[{i:D4}]Incorrect Spend Time: {watchTime.Ticks / 10.0d} us");
                    Console.ForegroundColor = temp;
                }
            }

            Console.WriteLine($"Run Done Spend Time: {totalTime.TotalMilliseconds} ms");

            File.WriteAllText(fileName, sb.ToString());
            File.AppendAllText(fileName, $"Total Spend Time: {totalTime.TotalMilliseconds} ms");
        }

        static void Case3(string fileName)
        {
            var serClient = server.Clients.ElementAt(0);
            var tcpClient = myClient[0];

            StringBuilder sb = new StringBuilder(10 * 1024 * 1024);

            var randomString = GetRandomString(1024);

            serClient.Write(randomString);
            var getStr = tcpClient.Read();

            TimeSpan totalTime = TimeSpan.FromTicks(0);
            for (int i = 0; i < 4096; i++)
            {
                randomString = GetRandomString(1024);

                DateTime startTime = DateTime.Now;

                serClient.Write(randomString);
                var getStr1 = tcpClient.Read();
                tcpClient.Write(getStr1);
                var getStr2 = serClient.Read();

                TimeSpan watchTime = DateTime.Now - startTime;
                totalTime += watchTime;
                if (getStr2 == randomString)
                {
                    //Console.WriteLine($"[{i:D4}]Correct Spend Time: {watchTime.Ticks / 10.0d} us");
                    LogTimeSpan(sb, i, watchTime);
                }
                else
                {
                    var temp = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[{i:D4}]Incorrect Spend Time: {watchTime.Ticks / 10.0d} us");
                    Console.ForegroundColor = temp;
                }
            }

            Console.WriteLine($"Run Done Spend Time: {totalTime.TotalMilliseconds} ms");

            File.WriteAllText(fileName, sb.ToString());
            File.AppendAllText(fileName, $"Total Spend Time: {totalTime.TotalMilliseconds} ms");
        }

        static void Case4(string fileName)
        {
            var serClient = server.Clients.ElementAt(0);
            var tcpClient = myClient[0];

            StringBuilder sb = new StringBuilder(10 * 1024 * 1024);

            var randomString = GetRandomString(1024);

            serClient.Write(randomString);
            var getStr = tcpClient.Read();

            TimeSpan totalTime = TimeSpan.FromTicks(0);
            for (int i = 0; i < 4096; i++)
            {
                randomString = GetRandomString(1024);

                DateTime startTime = DateTime.Now;

                tcpClient.Write(randomString);
                var getStr1 = serClient.Read();
                serClient.Write(getStr1);
                var getStr2 = tcpClient.Read();

                TimeSpan watchTime = DateTime.Now - startTime;
                totalTime += watchTime;
                if (getStr2 == randomString)
                {
                    //Console.WriteLine($"[{i:D4}]Correct Spend Time: {watchTime.Ticks / 10.0d} us");
                    LogTimeSpan(sb, i, watchTime);
                }
                else
                {
                    var temp = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[{i:D4}]Incorrect Spend Time: {watchTime.Ticks / 10.0d} us");
                    Console.ForegroundColor = temp;
                }
            }

            Console.WriteLine($"Run Done Spend Time: {totalTime.TotalMilliseconds} ms");

            File.WriteAllText(fileName, sb.ToString());
            File.AppendAllText(fileName, $"Total Spend Time: {totalTime.TotalMilliseconds} ms");
        }

        static void LogTimeSpan(StringBuilder sb, int index, TimeSpan ts)
        {
            sb.AppendLine($"{index}, {ts.Ticks / 10.0d} us");
        }

        static void StartServer()
        {
            server.OnClientConnect += Server_OnClientConnect;
            var lister = server.StartListener();
            
        }

        static void StopServer()
        {
            server.OnClientConnect -= Server_OnClientConnect;
            server.StopListener();
        }

        static async void GetClientIDs()
        {
            foreach(var client in server.Clients)
            {
                await client.WriteAsync($"ID?");
            }
        }

        private static void Server_OnClientConnect(TCPClient client)
        {
            Console.WriteLine($"Server Get Client: {client.ID}");
            client.OnReceive += ServerClient_OnReceive;
            //client.BeginRead();
        }

        private static void ServerClient_OnReceive(object sender, string message)
        {
            var client = sender as TCPClient;
            Console.WriteLine($"Server Rec Client\"{client.ID}\": {message}");
        }

        static async Task StartClient(params string[] ids)
        {
            foreach(var id in ids)
            {
                var client = new TCPClient(id);
                bool result = await client.ConnectAsync();
                if (result)
                {
                    Console.WriteLine($"Client Connect to Server Success");
                }
                else
                {
                    Console.WriteLine($"Client Connect to Server Fail");

                }
                client.OnReceive += Client_OnReceive;
                //client.BeginRead();
                myClient.Add(client);
            }
        }

        private static void Client_OnReceive(object sender, string message)
        {
            var client = sender as TCPClient;
        }

        public static string GetRandomString(int length)
        {
            var str = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            var next = new Random(DateTime.Now.GetHashCode());
            var builder = new StringBuilder(length + 1);
            for (var i = 0; i < length; i++)
            {
                builder.Append(str[next.Next(0, str.Length)]);
            }
            return builder.ToString();
        }
    }
}
