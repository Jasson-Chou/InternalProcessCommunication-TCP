using IPCLib.TCPIP;
using JcConsoleProgressBarLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InternalProcessCommunicationWithTCPDemo
{
    class Program
    {
        static TCPServer server;
        static List<TCPClient> myClient = new List<TCPClient>();
        static JcConsoleProgressBar ProgressBarInstance { get; set; }
        public const int TestCount = 4096;
        static void Main(string[] args)
        {
            server = new TCPServer("TestServer");
            int maxClient = 100;

            StartServer();
            foreach (var num in Enumerable.Range(0, maxClient))
            {
                StartClient("C-" + num).Wait();
            }
            bool isRunning = true;
            string folder = Path.Combine(Directory.GetCurrentDirectory(), "TestDatas");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            string baseFileName = Path.Combine(folder, "Case");
            double delayTime = 0.0d;
            
            while (isRunning)
            {
                ProgressBarInstance = new JcConsoleProgressBar();

                Console.WriteLine($"Press Number to Test{Environment.NewLine}" +
                $"1. Case1{Environment.NewLine}" +
                $"2. Case2{Environment.NewLine}" +
                $"3. Case3{Environment.NewLine}" +
                $"4. Case4{Environment.NewLine}" +
                $"5. Case5{Environment.NewLine}" + 
                $"Each Delay(ms): Delay Number => Delay 10.5 (unit:ms){Environment.NewLine}" + 
                "X to Exit");
                
                var readLine = Console.ReadLine().Trim().ToUpper();

                

                switch (readLine)
                {
                    case "1":
                        Case1(baseFileName + "1.csv", TimeSpan.FromMilliseconds(delayTime));
                        break;
                    case "2":
                        Case2(baseFileName + "2.csv", TimeSpan.FromMilliseconds(delayTime));
                        break;
                    case "3":
                        Case3(baseFileName + "3.csv", TimeSpan.FromMilliseconds(delayTime));
                        break;
                    case "4":
                        Case4(baseFileName + "4.csv", TimeSpan.FromMilliseconds(delayTime));
                        break;
                    case "5":
                        {
                            do
                            {
                                Console.WriteLine($"Input Client Count:(1 ~ {maxClient})");
                                Console.WriteLine($"Press X to Exit");
                                var getNumStr = Console.ReadLine();
                                if (int.TryParse(getNumStr, out int num))
                                {
                                    Case5(baseFileName + "5.csv", TimeSpan.FromMilliseconds(delayTime), num);
                                    break;
                                }
                                else if(getNumStr.Trim().ToUpper() == "X")
                                {
                                    break;
                                }
                                else
                                {
                                    Console.WriteLine($"Please Input Number:");
                                    continue;
                                }
                            } while (true);
                            break;
                        }
                    case "X":
                        isRunning = false;
                        break;

                    default:
                        Console.Clear();
                        if (readLine.Contains("Delay".ToUpper()))
                        {
                            var delayStr = readLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1];

                            if (double.TryParse(delayStr, out double delayValue))
                            {
                                delayTime = delayValue;

                                Console.WriteLine($"Input Delay Success.(Input Value: {delayStr})");
                            }
                            else
                            {
                                Console.WriteLine($"Input Delay Fail.(Input Error: {delayStr})");
                            }

                            continue;
                        }
                        else
                        {
                            Console.WriteLine($"Input Error: {readLine}");
                        }
                        break;

                }

                
                
                
                
                
            }

            

            Console.ReadKey();
        }

        static void Delay(TimeSpan delay)
        {
            var start = DateTime.Now;
            var diff = DateTime.Now - start;

            do
            {
                diff = DateTime.Now - start;
            } while (diff < delay);
            
        }

        static void Case1(string fileName, TimeSpan delay = default(TimeSpan))
        {
            var serClient = server.Clients.ElementAt(0);
            var tcpClient = myClient[0];

            StringBuilder sb = new StringBuilder(10 * 1024 * 1024);

            var randomString = GetRandomString(1024);

            serClient.Write(randomString);
            var getStr = tcpClient.Read();

            TimeSpan totalTime = TimeSpan.FromTicks(0);
            for (int i = 0; i < TestCount; i++)
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

                if (delay != default(TimeSpan)) Delay(delay);
                DrawTextProgressBar(nameof(Case1), i + 1, TestCount);
            }
            Console.WriteLine();
            Console.WriteLine($"Run Done Spend Time: {totalTime.TotalMilliseconds} ms");

            File.WriteAllText(fileName, sb.ToString());
            File.AppendAllText(fileName, $"Total Spend Time: {totalTime.TotalMilliseconds} ms");
        }

        static void Case2(string fileName, TimeSpan delay = default(TimeSpan))
        {
            var serClient = server.Clients.ElementAt(0);
            var tcpClient = myClient[0];

            StringBuilder sb = new StringBuilder(10 * 1024 * 1024);

            var randomString = GetRandomString(1024);

            serClient.Write(randomString);
            var getStr = tcpClient.Read();

            TimeSpan totalTime = TimeSpan.FromTicks(0);
            for (int i = 0; i < TestCount; i++)
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

                if (delay != default(TimeSpan)) Delay(delay);
                DrawTextProgressBar(nameof(Case2), i + 1, TestCount);
            }
            Console.WriteLine();
            Console.WriteLine($"Run Done Spend Time: {totalTime.TotalMilliseconds} ms");

            File.WriteAllText(fileName, sb.ToString());
            File.AppendAllText(fileName, $"Total Spend Time: {totalTime.TotalMilliseconds} ms");
        }

        static void Case3(string fileName, TimeSpan delay = default(TimeSpan))
        {
            var serClient = server.Clients.ElementAt(0);
            var tcpClient = myClient[0];

            StringBuilder sb = new StringBuilder(10 * 1024 * 1024);

            var randomString = GetRandomString(1024);

            serClient.Write(randomString);
            var getStr = tcpClient.Read();

            TimeSpan totalTime = TimeSpan.FromTicks(0);
            for (int i = 0; i < TestCount; i++)
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

                if (delay != default(TimeSpan)) Delay(delay);
                DrawTextProgressBar(nameof(Case3), i + 1, TestCount);

            }
            Console.WriteLine();
            Console.WriteLine($"Run Done Spend Time: {totalTime.TotalMilliseconds} ms");

            File.WriteAllText(fileName, sb.ToString());
            File.AppendAllText(fileName, $"Total Spend Time: {totalTime.TotalMilliseconds} ms");
        }

        static void Case4(string fileName, TimeSpan delay = default(TimeSpan))
        {
            var serClient = server.Clients.ElementAt(0);
            var tcpClient = myClient[0];

            StringBuilder sb = new StringBuilder(10 * 1024 * 1024);

            var randomString = GetRandomString(1024);

            serClient.Write(randomString);
            var getStr = tcpClient.Read();

            TimeSpan totalTime = TimeSpan.FromTicks(0);
            for (int i = 0; i < TestCount; i++)
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

                if (delay != default(TimeSpan)) Delay(delay);
                DrawTextProgressBar(nameof(Case4), i + 1, TestCount);
            }
            Console.WriteLine();
            Console.WriteLine($"Run Done Spend Time: {totalTime.TotalMilliseconds} ms");

            File.WriteAllText(fileName, sb.ToString());
            File.AppendAllText(fileName, $"Total Spend Time: {totalTime.TotalMilliseconds} ms");
        }

        static void Case5(string fileName, TimeSpan delay = default(TimeSpan), int clientCount = 1)
        {

            StringBuilder[] stringArr = new StringBuilder[clientCount];

            TimeSpan[] totalTimes = new TimeSpan[clientCount];
            var progressBaseTop = Console.CursorTop + 1;

            JcConsoleProgressCollection progressCollection = new JcConsoleProgressCollection();

            var CursorTop = Console.CursorTop;

            for(int cIndex = 0; cIndex < clientCount; cIndex++)
            {
                var progressBar = new JcConsoleProgressBar($"Client {cIndex}");
                progressBar.ConsoleTop = CursorTop + cIndex;
                progressBar.ConsoleLeft = 0;

                progressCollection.Add(progressBar);
            }

            Timer FPSTimer = new Timer((state) =>
            {
                var progresses = state as JcConsoleProgressCollection;
                progresses.ConsoleWriteAll();
            }, 
            progressCollection,
            Timeout.Infinite, (int)(1.0d / 30));
            

            Parallel.For(0, clientCount, cIndex =>
            {
                var serClient = server.Clients.ElementAt(cIndex);
                var tcpClient = myClient[cIndex];
                stringArr[cIndex] = new StringBuilder(10 * 1024 * 1024);
                StringBuilder sb = stringArr[cIndex];

                var randomString = GetRandomString(1024);

                serClient.Write(randomString);
                var getStr = tcpClient.Read();

                totalTimes[cIndex] = TimeSpan.FromTicks(0);

                var progress = progressCollection[cIndex];

                for (int i = 0; i < TestCount; i++)
                {
                    randomString = GetRandomString(1024);

                    DateTime startTime = DateTime.Now;

                    tcpClient.Write(randomString);
                    var getStr1 = serClient.Read();
                    serClient.Write(getStr1);
                    var getStr2 = tcpClient.Read();

                    TimeSpan watchTime = DateTime.Now - startTime;
                    totalTimes[cIndex] += watchTime;
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

                    if (delay != default(TimeSpan)) Delay(delay);
                    //DrawTextProgressBar($"Client{cIndex}", cIndex, i + 1, TestCount, progressBaseTop);
                    progress.Update(i + 1, TestCount);
                }
            });

            FPSTimer.Dispose();

            progressCollection.ConsoleWriteAll();

            Console.CursorTop = CursorTop + clientCount;
            Console.CursorLeft = 0;

            for (int cIndex = 0; cIndex < clientCount; cIndex++)
            {
                double ms = totalTimes[cIndex].TotalMilliseconds;
                double avg = ms / TestCount;
                Console.WriteLine($"Client[{cIndex}] Run Done Spend Time: {ms} ms, Avg: {avg} ms");
            }

            string[][] logResults = new string[clientCount][];

            StringBuilder logResultSB = new StringBuilder(1024 * (TestCount + 1));
            

            for(int cIndex = 0; cIndex < clientCount; cIndex++)
            {
                logResults[cIndex] = stringArr[cIndex].ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                logResultSB.Append($"Client {cIndex}");

                if (cIndex < clientCount - 1)
                {
                    logResultSB.Append(",");
                }
            }


            logResultSB.AppendLine();
            for (int i = 0; i < TestCount; i++)
            {
                for (int cIndex = 0; cIndex < clientCount; cIndex++)
                {
                    logResultSB.Append(logResults[cIndex][i]);

                    if(cIndex < clientCount - 1)
                    {
                        logResultSB.Append(",");
                    }
                }

                logResultSB.AppendLine();
            }

            File.WriteAllText(fileName, logResultSB.ToString());
        }

        static void LogTimeSpan(StringBuilder sb, int index, TimeSpan ts)
        {
            sb.AppendLine($"{ts.Ticks / 10.0d} us");
        }

        static void StartServer()
        {
            server.OnClientConnect += Server_OnClientConnect;
            var lister = server.StartListener();
            while (!server.Created) Thread.Sleep(1000);
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
                var client = new TCPClient(id, "TestServer");
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

        static void DrawTextProgressBar(string header, int progress, int total)
        {
            ProgressBarInstance.Header = header;
            ProgressBarInstance.Update(progress, total);
            var progressStr = ProgressBarInstance.ConsoleWrite();
            Trace.WriteLine(progressStr);
        }

    }
}
