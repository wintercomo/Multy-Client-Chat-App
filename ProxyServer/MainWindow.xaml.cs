using ClassLibrary1;
using ProxyClasses;
using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ProxyServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<LogItem> LogItems = new ObservableCollection<LogItem>();
        ChatAppFunctions chatApp = new ChatAppFunctions();
        TcpListener tcpListner;
        NetworkStream clientStream;
        TcpClient tcpClient;
        ProxySettings settings;
        public MainWindow()
        {
            InitializeComponent();
            settings = new ProxySettings {
                Port = 26, CacheTimeout= 60000, BufferSize=2,
                CheckModifiedContent =true, ContentFilterOn=true,
                BasicAuthOn = false, AllowChangeHeaders= true,
                LogRequestHeaders = true, LogContentIn= true,
                LogContentOut=true, LogCLientInfo = true,
                ServerRunning=false
            };
            this.DataContext = settings;
            LogItems.Add(new LogItem(LogItem.REQUEST) {
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
                if (settings.ServerRunning)
                {
                    StopProxyServer();
                    return;
                }
                tcpListner = new TcpListener(IPAddress.Any, 8090);
                tcpListner.Start();
                LogItems.Add(new LogItem(LogItem.REQUEST) { LogItemInfo = "Listening for HTTP REQUEST" });
                settings.ServerRunning = true;
                // keep looking for a request
                while (true)
                {
                    //Wait for a client aka a request
                    tcpClient = await tcpListner.AcceptTcpClientAsync();
                    clientStream = tcpClient.GetStream();
                    // get the request details
                    string requestInfo = await chatApp.CreateMessageFromReading(clientStream, settings.BufferSize);
                    // send request if no spam request
                    if (!requestInfo.Contains("detectportal"))
                    {
                        LogItems.Add(new LogItem(LogItem.REQUEST) { LogItemInfo = requestInfo });
                        //UpdateUI($"[REQUEST] \r\n {requestInfo}");
                        await HandleHttpRequest(requestInfo);
                        clientStream.Close();
                    }
                }
            }
            catch (HttpListenerException)
            {
                LogItems.Add(new LogItem(LogItem.MESSAGE) { LogItemInfo = "Server stopped running Due to ERROR" });
            }
            catch (ObjectDisposedException err)
            {
                LogItems.Add(new LogItem(LogItem.MESSAGE) { LogItemInfo = "Connot get request. server stopped, ERROR LOG: \r\n " + err.Message });
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
                ReceiveTimeout = settings.CacheTimeout,
                ReceiveBufferSize = settings.BufferSize
            };
            NetworkStream stream = tcp.GetStream();
            // generate request
            byte[] httpRequest = Encoding.ASCII.GetBytes(httpRequestString);
            stream.Write(httpRequest, 0, httpRequest.Length);
            LogItems.Add(new LogItem(LogItem.MESSAGE) { LogItemInfo = httpRequestString });
            // get response from request and send it back to client
            string responseData = await chatApp.CreateMessageFromReading(stream, settings.BufferSize);
            LogItems.Add(new LogItem(LogItem.RESPONSE) { LogItemInfo = responseData });
            byte[] bytesToSend = Encoding.ASCII.GetBytes(responseData);
            await clientStream.WriteAsync(bytesToSend, 0, bytesToSend.Length);
        }

        private void StopProxyServer()
        {
            LogItems.Add(new LogItem(LogItem.MESSAGE) { LogItemInfo = "Stopping proxy Server..." });
            tcpListner.Stop();
            settings.ServerRunning = false;
            LogItems.Add(new LogItem(LogItem.MESSAGE) { LogItemInfo = "Proxy server Stopped Running" });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LogItems.Clear();
        }
    }
}
