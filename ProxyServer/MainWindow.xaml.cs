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
                Port = 8090, CacheTimeout= 60000, BufferSize=1000,
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
                await ListenForHttpRequest();
                return;
            }
            catch (HttpListenerException)
            {
                UpdateUIWithLogItem(new LogItem(LogItem.MESSAGE, settings) { LogItemInfo = "Server stopped running Due to ERROR" });
            }
            catch (ObjectDisposedException err)
            {
                UpdateUIWithLogItem(new LogItem(LogItem.MESSAGE, settings) { LogItemInfo = "Connot get request. server stopped, ERROR LOG: \r\n " + err.Message });
            }
            catch (UriFormatException err)
            {
                UpdateUIWithLogItem(new LogItem(LogItem.MESSAGE, settings) { LogItemInfo = "Cannot parse host IP: \r\n " + err.Message });
            }
            catch (ArgumentException err)
            {
                UpdateUIWithLogItem(new LogItem(LogItem.MESSAGE, settings) { LogItemInfo = "Argument Exception!: \r\n " + err.Message });
            } 
            finally
            {
                StopProxyServer();
            }
        }

        private async Task ListenForHttpRequest()
        {
            while (true)
            {
                //Wait for a client aka a request
                tcpClient = await tcpListner.AcceptTcpClientAsync();
                clientStream = tcpClient.GetStream();
                string requestInfo = await chatApp.CreateMessageFromReading(clientStream, settings.BufferSize);
                // firefox spam requests
                if (!requestInfo.Contains("detectportal"))
                {
                    clientRequest = new LogItem(LogItem.REQUEST, settings) { LogItemInfo = requestInfo };
                    if (settings.LogContentIn)
                    {
                        UpdateUIWithLogItem(clientRequest);
                    }
                    await HandleHttpRequest(clientRequest.HttpString);
                    clientStream.Close();
                }
            }
        }

        private async Task HandleHttpRequest(string httpRequestString)
        {
            // if request is known. Send the known response back
            if (cacher.RequestKnown(clientRequest.Method))
            {
                string knownResponse = cacher.GetKnownResponse(clientRequest.Method);
                serverResponse = new LogItem(LogItem.CACHED_RESPONSE, settings) { LogItemInfo = knownResponse };
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
            serverResponse = new LogItem(LogItem.RESPONSE, settings) { LogItemInfo = responseData };
            if (settings.LogContentIn)
            {
                UpdateUIWithLogItem(serverResponse);
            }
            cacher.addRequest(clientRequest.Method, responseData);
            byte[] bytesToSend = Encoding.ASCII.GetBytes(responseData);
            await clientStream.WriteAsync(bytesToSend, 0, bytesToSend.Length);
        }

        private async Task<string> DoProxyRequest(string httpRequestString)
        {
            string data = getBetween(httpRequestString, "http://", "/");
            Uri baseUri = new Uri("http://" + data);
            TcpClient serverClient = new TcpClient(baseUri.Host, baseUri.Port)
            {
                NoDelay = false,
                ReceiveTimeout = settings.CacheTimeout,
                ReceiveBufferSize = settings.BufferSize
            };
            NetworkStream stream = serverClient.GetStream();
            // generate request
            byte[] httpRequest = Encoding.ASCII.GetBytes(httpRequestString);
            stream.Write(httpRequest, 0, httpRequest.Length);
            proxyRequest = new LogItem(LogItem.PROXY_REQUEST, settings) { LogItemInfo = httpRequestString }; //CHECK IF NEEDED
            if (settings.LogContentIn)
            {
                UpdateUIWithLogItem(proxyRequest);
            }
            // get response from request and send it back to client
            string responseData = await chatApp.CreateMessageFromReading(stream, settings.BufferSize);
            return responseData;
        }
        public static string getBetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                return "";
            }
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
