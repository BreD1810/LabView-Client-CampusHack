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
        static AppDomain _appDomain;

        static string _machineName = Environment.MachineName;

        static void Main(string[] args)
        {
            _appDomain = AppDomain.CurrentDomain;
            Debug.WriteLine("Client started.");
            Debug.WriteLine($"Machine Name: {_machineName}");
            do
            {
                Connect(_serverAddress).Wait();
                Thread.Sleep(60000);
            }
            while (true);

        }

        public static async Task Connect(string uri)
        {

            ClientWebSocket webSocket = null;

            try
            {
                webSocket = new ClientWebSocket();
                await webSocket.ConnectAsync(new Uri(uri), CancellationToken.None);
                Debug.WriteLine($"Connection established with {_serverAddress}");
                _appDomain.ProcessExit += (object sender, EventArgs e) =>
                {
                    if (webSocket != null)
                    {
                        if (webSocket.State == WebSocketState.Open)
                        {
                            Debug.WriteLine("Closing connection to server...");
                            try
                            {
                                webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closed.", CancellationToken.None).Wait();
                            }
                            catch(Exception ex) { Debug.WriteLine(ex); }
                        }
                        Debug.WriteLine("Disposing resources...");
                        try { webSocket.Dispose(); } catch(Exception ex) { Debug.WriteLine(ex); }
                    }
                    Debug.WriteLine("Safely exiting program.");
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
                    try { await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client Disconnected.", CancellationToken.None); }
                    catch (Exception e) { Debug.WriteLine(e); }

                    try { webSocket.Dispose(); } catch (Exception e) { Debug.WriteLine(e); }
                }
                Debug.WriteLine("WebSocket closed.");
            }
        }

        private static async Task Send(ClientWebSocket webSocket)
        {

            byte[] buffer = Encoding.UTF8.GetBytes(_machineName);
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
