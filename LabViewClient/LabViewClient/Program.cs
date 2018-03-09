using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

namespace LabViewClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Debug.WriteLine("Client Started.");
            WebRequest request = WebRequest.Create("http://localhost:8080");
            ((HttpWebRequest)request).UserAgent = ".NET Framework Example Client";
            request.Method = "POST";
            request.ContentType = "text";
            Stream requestStream = request.GetRequestStream();
            var data = Encoding.UTF8.GetBytes("I'm a Client, and I'm Alive!!!");
            requestStream.Write(data, 0, data.Length);
            requestStream.Close();
            WebResponse response = request.GetResponse();
            Console.WriteLine(response);
            while (Console.KeyAvailable)
                Console.ReadKey(true);
            Console.WriteLine("\r\nPress any key to continue...");
            Console.ReadKey();
            
        }
    }
}
