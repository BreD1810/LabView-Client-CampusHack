using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LabViewClient
{
    class Program
    {
        const string _serverAddress = "localhost";
        const int _serverPort = 1337;
        static void Main(string[] args)
        {
            Debug.WriteLine("Client Started.");
            var tcpClient = new TcpClient(_serverAddress, _serverPort);
            Debug.WriteLine($"TCP Connection established with {_serverAddress}");
            byte[] msg = Encoding.UTF8.GetBytes("I'm a Client and I'm Alive!!!!!!\n");
            tcpClient.Client.Send(msg);
            Debug.WriteLine("Message Sent...");
            byte[] responseBuffer = new byte[1024];
            int bytesRec = tcpClient.Client.Receive(responseBuffer);
            string response = Encoding.UTF8.GetString(responseBuffer, 0, bytesRec);
            Debug.WriteLine($"Response received: \"{response}\"");

            //WebRequest request = WebRequest.Create("http://localhost:8080");
            //((HttpWebRequest)request).UserAgent = ".NET Framework Example Client";
            //request.Method = "POST";
            //request.ContentType = "text";
            //Stream requestStream = request.GetRequestStream();
            //var data = Encoding.UTF8.GetBytes("I'm a Client, and I'm Alive!!!");
            //requestStream.Write(data, 0, data.Length);
            //requestStream.Close();
            //WebResponse response = request.GetResponse();
            //Console.WriteLine(response);
            while (Console.KeyAvailable)
                Console.ReadKey(true);
            Console.WriteLine("\r\nPress any key to continue...");
            Console.ReadKey();


        }
    }
}
