using System.Runtime.CompilerServices;
using System.IO;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;

namespace ServicesChecker
{
    class Program
    {
        private const string defaultName = "ServerList.txt";
        static void Main(string[] args)
        {
            string dirPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            var listFileName = defaultName;
            if (args.Length >= 1)
            {
                listFileName = args[0];
            }
            var filePath = Path.Combine(dirPath, listFileName);

            if (File.Exists(filePath))
            {
                var serverList = new List<ServerInfo>();
                using (var streamReader = new StreamReader(filePath))
                {
                    string line = string.Empty;
                    string[] temp;
                    string ip;
                    int port;
                    while (!streamReader.EndOfStream)
                    {
                        line = streamReader.ReadLine();
                        if (line.Contains("#") == false)
                        {
                            temp = line.Split(":");
                            if (temp.Length == 2)
                            {
                                ip = temp[0];
                                if (int.TryParse(temp[1], out port))
                                {
                                    var serverInfo = new ServerInfo()
                                    {
                                        IP = ip,
                                        Port = port
                                    };
                                    serverList.Add(serverInfo);
                                }
                            }
                        }
                    }
                }
                var count = serverList.Count;
                Console.WriteLine($"Count:{count}");
                for (int i = 0; i < count; i++)
                {
                    var item = serverList[i];
                    var ip = item.IP;
                    var port = item.Port;
                    var timeout = 3000;
                    var result = Check(ip, port, timeout);
                    Console.WriteLine($"({i + 1}/{count}) Check {ip}:{port} Result:{(result ? "Success" : "Fail")}");
                }
            }
            else
            {
                Console.WriteLine("List File No Exists");
                var exampleFilePath = Path.Combine(dirPath, defaultName);
                using (var sw = new StreamWriter(exampleFilePath, false))
                {
                    sw.WriteLine($"#Example");
                    sw.WriteLine($"#127.0.0.1:8080");
                }
                Console.WriteLine($"Make Example File in {exampleFilePath}");
            }
            
            Console.WriteLine();
            Console.WriteLine($"Press Any Key End Application");
            Console.ReadKey();
        }

        static bool Check(string IPStr, int Port, int Timeout)
        {
            bool success = false;
            using (TcpClient tcpClient = new TcpClient())
            {
                try
                {
                    tcpClient.Connect(IPStr, Port);
                    success = true;
                }
                catch (Exception)
                {

                }
            }
            return success;
        }

        private struct ServerInfo
        {
            public string IP;
            public int Port;
        }
    }


}
