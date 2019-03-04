using ClassLibrary1;
using ProxyClasses;
using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
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
        Cacher cacher = new Cacher();
        TcpListener tcpListner;
        NetworkStream clientStream;
        TcpClient tcpClient;
        public ProxySettings settings;
        LogItem clientRequest;
        LogItem proxyRequest;
        LogItem serverResponse;
        public MainWindow()
        {
            InitializeComponent();
            settings = new ProxySettings {
                Port = 26, CacheTimeout= 60000, BufferSize=2,
                CheckModifiedContent =true, ContentFilterOn=true,
                BasicAuthOn = false, AllowChangeHeaders= true,
                LogRequestHeaders = false, LogContentIn= true,
                LogContentOut=true, LogCLientInfo = true,
                ServerRunning=false
            };
            this.DataContext = settings;
            LogItems.Add(new LogItem(LogItem.MESSAGE, settings) {
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
                LogItems.Add(new LogItem(LogItem.MESSAGE, settings) { LogItemInfo = "Listening for HTTP REQUEST" });
                settings.ServerRunning = true;
                while (true)
                {
                    //Wait for a client aka a request
                    tcpClient = await tcpListner.AcceptTcpClientAsync();
                    clientStream = tcpClient.GetStream();
                    string requestInfo = await chatApp.CreateMessageFromReading(clientStream, settings.BufferSize);
                    if (!requestInfo.Contains("detectportal"))
                    {
                        HttpRequest requestObject = new HttpRequest(requestInfo);
                        clientRequest = new LogItem(LogItem.REQUEST, settings) { LogItemInfo = requestObject.ToString() };
                        LogItems.Add(clientRequest);
                        await HandleHttpRequest(requestInfo);
                        clientStream.Close();
                    }
                }
            }
            catch (HttpListenerException)
            {
                LogItems.Add(new LogItem(LogItem.MESSAGE, settings) { LogItemInfo = "Server stopped running Due to ERROR" });
            }
            catch (ObjectDisposedException err)
            {
                LogItems.Add(new LogItem(LogItem.MESSAGE, settings) { LogItemInfo = "Connot get request. server stopped, ERROR LOG: \r\n " + err.Message });
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task HandleHttpRequest(string httpRequestString)
        {
            // if request is known. Send the known response back
            if (cacher.RequestKnown(clientRequest.Method))
            {
                HttpRequest knownResponse = cacher.GetKnownResponse(clientRequest.Method);
                serverResponse = new LogItem(LogItem.RESPONSE, settings) { LogItemInfo = knownResponse.ToString() };
                LogItems.Add(serverResponse);
                byte[] messageInBytes = Encoding.ASCII.GetBytes(knownResponse.ToString());
                await clientStream.WriteAsync(messageInBytes, 0, messageInBytes.Length);
                return;
            }
            // Get the actual response
            string responseData = await DoProxyRequest(httpRequestString);
            HttpRequest httpRequestObject = new HttpRequest(responseData);
            serverResponse = new LogItem(LogItem.RESPONSE, settings) { LogItemInfo = httpRequestObject.ToString() };
            LogItems.Add(serverResponse);
            cacher.addRequest(clientRequest.Method, httpRequestObject);
            byte[] bytesToSend = Encoding.ASCII.GetBytes(responseData);
            await clientStream.WriteAsync(bytesToSend, 0, bytesToSend.Length);
        }

        private async Task<string> DoProxyRequest(string httpRequestString)
        {
            TcpClient serverClient = new TcpClient("localhost", 8080)
            {
                NoDelay = false,
                ReceiveTimeout = settings.CacheTimeout,
                ReceiveBufferSize = settings.BufferSize
            };
            NetworkStream stream = serverClient.GetStream();
            // generate request
            byte[] httpRequest = Encoding.ASCII.GetBytes(httpRequestString);
            stream.Write(httpRequest, 0, httpRequest.Length);
            proxyRequest = new LogItem(LogItem.REQUEST, settings) { LogItemInfo = httpRequestString }; //CHECK IF NEEDED
            LogItems.Add(proxyRequest);
            // get response from request and send it back to client
            string responseData = await chatApp.CreateMessageFromReading(stream, settings.BufferSize);
            return responseData;
        }

        private void StopProxyServer()
        {
            LogItems.Add(new LogItem(LogItem.MESSAGE, settings) { LogItemInfo = "Stopping proxy Server..." });
            tcpListner.Stop();
            settings.ServerRunning = false;
            LogItems.Add(new LogItem(LogItem.MESSAGE, settings) { LogItemInfo = "Proxy server Stopped Running" });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LogItems.Clear();
        }
    }
}
