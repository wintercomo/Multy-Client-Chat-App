using ClassLibrary1;
using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProxyServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TcpListener tcpListner;
        ChatAppFunctions chatApp = new ChatAppFunctions();
        NetworkStream clientStream;
        TcpClient tcpClient;
        private ObservableCollection<LogItem> LogItems = new ObservableCollection<LogItem>();
        public MainWindow()
        {
            InitializeComponent();
            LogItems.Add(new LogItem() {
                LogItemInfo = "Log van:\r\n" +
                "   * request headers\r\n" +
                "   * Response headers\r\n" +
                "   * content in\r\n" +
                "   * content uit\r\n" +
                "   * Client (Which browser is connected)"
            });

            logListBox.ItemsSource = LogItems;
        }
        private async void BtnStartStopProxy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // stop server if running
                if (tcpListner != null)
                {
                    StopProxyServer();
                    return;
                }
                UpdateUI("Starting server....");
                tcpListner = new TcpListener(IPAddress.Any, 8090);
                tcpListner.Start();
                UpdateUI("Listening for HTTP Requests");
                // keep looking for a request
                while (true)
                {
                    //Wait for a client aka a request
                    tcpClient = await tcpListner.AcceptTcpClientAsync();
                    clientStream = tcpClient.GetStream();
                    // get the request details
                    string requestInfo = await chatApp.CreateMessageFromReading(clientStream, 1024);
                    // send request if no spam request
                    if (!requestInfo.Contains("detectportal"))
                    {
                        UpdateUI($"[REQUEST] \r\n {requestInfo}");
                        await HandleHttpRequest(requestInfo);
                        clientStream.Close();
                    }
                }
            }
            catch (HttpListenerException)
            {
                UpdateUI("Server stopped running! ERROR");
            }
            catch (ObjectDisposedException err)
            {
                UpdateUI($"Connot get request. server stopped, ERROR LOG: \r\n " + err.Message);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task HandleHttpRequest(string httpRequestString)
        {
            // generate a client to user for getting the actual response
            TcpClient tcp = new TcpClient("localhost", 8080)
            {
                NoDelay = false,
                ReceiveTimeout = 60000,
                ReceiveBufferSize = 25000
            };
            NetworkStream stream = tcp.GetStream();
            // generate request
            byte[] httpRequest = Encoding.ASCII.GetBytes(httpRequestString.ToString());
            UpdateUI($"[PROXY] \r\n{ httpRequestString}");
            stream.Write(httpRequest, 0, httpRequest.Length);
            // generate response
            byte[] httpResponse = new byte[tcp.ReceiveBufferSize];
            int lastreceive = stream.Read(httpResponse, 0, tcp.ReceiveBufferSize);
            string responseData = Encoding.ASCII.GetString(httpResponse, 0, tcp.ReceiveBufferSize);
            // Show response
            UpdateUI($"[RESPONSE] \r\n {responseData}");
            byte[] bytesToSend = Encoding.ASCII.GetBytes(responseData);
            await clientStream.WriteAsync(bytesToSend, 0, bytesToSend.Length);
        }

        private void UpdateUI(String logMessage)
        {
            LogItems.Add(new LogItem() { LogItemInfo = logMessage });
        }

        private void StopProxyServer()
        {
            UpdateUI("Stopping proxy Server...");
            tcpListner.Stop();
            UpdateUI("Server stopped!");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LogItems.Clear();
        }
    }
}
