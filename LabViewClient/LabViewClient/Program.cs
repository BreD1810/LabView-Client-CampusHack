using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LabViewClient
{
    class Program
    {
        private static object consoleLock = new object();
        private const int sendChunkSize = 256;
        private const int receiveChunkSize = 256;
        private const bool verbose = true;
        private static readonly TimeSpan delay = TimeSpan.FromMilliseconds(30000);
        static UTF8Encoding encoder = new UTF8Encoding();

        const string _serverAddress = "ws://labview.me:8080/LabView/websocketendpoint";


        static string _machineName = Environment.MachineName;
        static void Main(string[] args)
        {

            Connect(_serverAddress).Wait();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

            //OLD CODE THAT MAY STILL BE USEFUL

                //Debug.WriteLine("Client Started.");
                //Debug.WriteLine($"Machine Name: {_machineName}");
                ////var tcpClient = new TcpClient(_serverAddress, _serverPort);

                //Debug.WriteLine($"TCP Connection established with {_serverAddress}");
                //byte[] msg = Encoding.UTF8.GetBytes($"<MachineName>{_machineName}\n");
                ////tcpClient.Client.Send(msg);
                //Debug.WriteLine("Message Sent...");
                //byte[] responseBuffer = new byte[1024];
                //int bytesRec = tcpClient.Client.Receive(responseBuffer);
                //string response = Encoding.UTF8.GetString(responseBuffer, 0, bytesRec);
                //Debug.WriteLine($"Response received: \"{response}\"");
                //Thread.Sleep(60000);

        }

        public static async Task Connect(string uri)
        {
            ClientWebSocket webSocket = null;

            try
            {
                webSocket = new ClientWebSocket();
                await webSocket.ConnectAsync(new Uri(uri), CancellationToken.None);
                await Task.WhenAll(Receive(webSocket), Send(webSocket));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex);
            }
            finally
            {
                if (webSocket != null)
                    webSocket.Dispose();
                Console.WriteLine();

                lock (consoleLock)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("WebSocket closed.");
                    Console.ResetColor();
                }
            }
        }

        private static async Task Send(ClientWebSocket webSocket)
        {

            //byte[] buffer = encoder.GetBytes("{\"op\":\"blocks_sub\"}"); //"{\"op\":\"unconfirmed_sub\"}");
            byte[] buffer = encoder.GetBytes("{\"op\":\"unconfirmed_sub\"}");
            await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);

            while (webSocket.State == WebSocketState.Open)
            {
                LogStatus(false, buffer, buffer.Length);
                await Task.Delay(delay);
            }
        }

        private static async Task Receive(ClientWebSocket webSocket)
        {
            byte[] buffer = new byte[receiveChunkSize];
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                }
                else
                {
                    LogStatus(true, buffer, result.Count);
                }
            }
        }

        private static void LogStatus(bool receiving, byte[] buffer, int length)
        {
            lock (consoleLock)
            {
                Console.ForegroundColor = receiving ? ConsoleColor.Green : ConsoleColor.Gray;
                //Console.WriteLine("{0} ", receiving ? "Received" : "Sent");

                if (verbose)
                    Console.WriteLine(encoder.GetString(buffer));

                Console.ResetColor();
            }
        }
    }

}
