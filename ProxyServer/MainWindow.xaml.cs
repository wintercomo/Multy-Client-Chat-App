using ClassLibrary1;
using ProxyClasses;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ProxyServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ProxyserverWindow : Window
    {
        ObservableCollection<LogItem> LogItems = new ObservableCollection<LogItem>();
        ChatAppFunctions chatApp = new ChatAppFunctions();
        Cacher cacher = new Cacher();
        TcpListener tcpListner;
        NetworkStream clientStream;
        TcpClient tcpClient;
        ProxySettings settings;
        LogItem clientRequest;
        LogItem proxyRequest;
        LogItem serverResponse;
        public ProxyserverWindow()
        {
            InitializeComponent();
            // Bind the new data source to the myText TextBlock control's Text dependency property.
            settings = new ProxySettings {
                Port = 8090, CacheTimeout= 60000, BufferSize=2,
                CheckModifiedContent =true, ContentFilterOn=true,
                BasicAuthOn = false, AllowChangeHeaders= true,
                LogRequestHeaders = false, LogContentIn= true,
                LogContentOut=true, LogCLientInfo = true,
                ServerRunning=false
            };
            this.DataContext = settings;
            UpdateUIWithLogItem(new LogItem(LogItem.MESSAGE, settings) {
                LogItemInfo = "Log van:\r\n" +
                "   * request headers\r\n" +
                "   * Response headers\r\n" +
                "   * content in\r\n" +
                "   * content uit\r\n" +
                "   * Client (Which browser is connected)"
            });
            logListBox.ItemsSource = LogItems;
        }

        private void UpdateUIWithLogItem(LogItem logItem)
        {
           
           LogItems.Add(logItem);
         
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
                tcpListner = new TcpListener(IPAddress.Any, settings.Port);
                tcpListner.Start();
                UpdateUIWithLogItem(new LogItem(LogItem.MESSAGE, settings) { LogItemInfo = "Listening for HTTP REQUEST" });
                settings.ServerRunning = true;
                while (true)
                {
                    //Wait for a client aka a request
                    tcpClient = await tcpListner.AcceptTcpClientAsync();
                    clientStream = tcpClient.GetStream();
                    string requestInfo = await chatApp.CreateMessageFromReading(clientStream, settings.BufferSize);
                    // firefox spam requests
                    if (!requestInfo.Contains("detectportal"))
                    {
                        HttpRequest requestObject = new HttpRequest(requestInfo);
                        clientRequest = new LogItem(LogItem.REQUEST, settings) { LogItemInfo = "CLIENT REQUEST " + requestObject.ToString() };
                        if (settings.LogContentIn)
                        {
                            UpdateUIWithLogItem(clientRequest);
                        }
                        await HandleHttpRequest(requestInfo);
                        clientStream.Close();
                    }
                }
            }
            catch (HttpListenerException)
            {
                UpdateUIWithLogItem(new LogItem(LogItem.MESSAGE, settings) { LogItemInfo = "Server stopped running Due to ERROR" });
            }
            catch (ObjectDisposedException err)
            {
                UpdateUIWithLogItem(new LogItem(LogItem.MESSAGE, settings) { LogItemInfo = "Connot get request. server stopped, ERROR LOG: \r\n " + err.Message });
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
                serverResponse = new LogItem(LogItem.RESPONSE, settings) { LogItemInfo = "CACHED RESPONSE " + knownResponse.ToString() };
                if (settings.LogContentOut)
                {
                    UpdateUIWithLogItem(serverResponse);
                }
                byte[] messageInBytes = Encoding.ASCII.GetBytes(knownResponse.ToString());
                await clientStream.WriteAsync(messageInBytes, 0, messageInBytes.Length);
                return;
            }
            // Get the actual response
            string responseData = await DoProxyRequest(httpRequestString);
            HttpRequest httpResponseObject = new HttpRequest(responseData);
            serverResponse = new LogItem(LogItem.RESPONSE, settings) { LogItemInfo = "SERVER RESPONSE " + httpResponseObject.ToString() };
            if (settings.LogContentIn)
            {
                UpdateUIWithLogItem(serverResponse);
            }
            cacher.addRequest(clientRequest.Method, httpResponseObject);
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
            proxyRequest = new LogItem(LogItem.REQUEST, settings) { LogItemInfo = "PROXY REQUEST " + httpRequestString }; //CHECK IF NEEDED
            if (settings.LogContentIn)
            {
                UpdateUIWithLogItem(proxyRequest);
            }
            // get response from request and send it back to client
            string responseData = await chatApp.CreateMessageFromReading(stream, settings.BufferSize);
            return responseData;
        }

        private void StopProxyServer()
        {
            UpdateUIWithLogItem(new LogItem(LogItem.MESSAGE, settings) { LogItemInfo = "Stopping proxy Server..." });
            tcpListner.Stop();
            settings.ServerRunning = false;
            UpdateUIWithLogItem(new LogItem(LogItem.MESSAGE, settings) { LogItemInfo = "Proxy server Stopped Running" });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LogItems.Clear();
        }
    }
}
