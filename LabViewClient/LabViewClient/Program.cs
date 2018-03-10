using System;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LabViewClient
{
    class Program
    {
        private static readonly TimeSpan Delay = TimeSpan.FromMilliseconds(30000);

        const string _serverAddress = "ws://labview.me:8080/LabView/websocketendpoint";

        static string _machineName = Environment.MachineName;

        static void Main(string[] args)
        {
            Debug.WriteLine("Client started.");
            Debug.WriteLine($"Machine Name: {_machineName}");
            Connect(_serverAddress).Wait();

        }

        public static async Task Connect(string uri)
        {
            ClientWebSocket webSocket = null;

            try
            {
                webSocket = new ClientWebSocket();
                await webSocket.ConnectAsync(new Uri(uri), CancellationToken.None);
                Debug.WriteLine($"Connection established with {_serverAddress}");
                AppDomain.CurrentDomain.ProcessExit += (object sender, EventArgs e) =>
                {
                    Task.WaitAll(webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closed.", CancellationToken.None));
                    webSocket.Dispose();
                };
                await Task.WhenAll(Receive(webSocket), Send(webSocket));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception: {0}", ex);
            }
            finally
            {
                if (webSocket != null)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client Disconnected.", CancellationToken.None);
                    webSocket.Dispose();
                }
                Debug.WriteLine("WebSocket closed.");
            }
        }

        private static async Task Send(ClientWebSocket webSocket)
        {

            byte[] buffer = Encoding.UTF8.GetBytes($"Brad");
            await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            Debug.WriteLine("Message sent.");
        }

        private static async Task Receive(ClientWebSocket webSocket)
        {
            byte[] buffer = new byte[1024];
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                }
                else
                {
                    HandleReceivedData(Encoding.UTF8.GetString(buffer, 0, result.Count));
                }
            }
        }

        private static void HandleReceivedData(string data)
        {
            Debug.WriteLine($"Data received: \"{data}\"");
            //TODO: Handle recieved data!!!
        }
    }

}
