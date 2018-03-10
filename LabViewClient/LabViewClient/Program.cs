using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace LabViewClient
{
    class Program
    {
        const string _serverAddress = "localhost";
        const int _serverPort = 1337;

        static string _machineName = Environment.MachineName;
        static void Main(string[] args)
        {
            while (true)
            {
                Debug.WriteLine("Client Started.");
                Debug.WriteLine($"Machine Name: {_machineName}");
                var tcpClient = new TcpClient(_serverAddress, _serverPort);
                Debug.WriteLine($"TCP Connection established with {_serverAddress}");
                byte[] msg = Encoding.UTF8.GetBytes($"<MachineName>{_machineName}\n");
                tcpClient.Client.Send(msg);
                Debug.WriteLine("Message Sent...");
                byte[] responseBuffer = new byte[1024];
                int bytesRec = tcpClient.Client.Receive(responseBuffer);
                string response = Encoding.UTF8.GetString(responseBuffer, 0, bytesRec);
                Debug.WriteLine($"Response received: \"{response}\"");
                Thread.Sleep(60000);
            }

            //while (Console.KeyAvailable)
            //    Console.ReadKey(true);
            //Console.WriteLine("\r\nPress any key to continue...");
            //Console.ReadKey();
        }
    }
}
