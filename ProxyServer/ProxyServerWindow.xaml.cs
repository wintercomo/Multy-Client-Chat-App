using ClassLibrary1;
using ProxyClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using StreamReader = ProxyClasses.StreamReader;

namespace ProxyServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ProxyserverWindow : Window
    {
        readonly ObservableCollection<HttpRequest> LogItems = new ObservableCollection<HttpRequest>();
        Cacher cacher = new Cacher();
        readonly StreamReader streamReader = new StreamReader();
        TcpListener tcpListner;
        ProxySettingsViewModel settings;
        HttpRequest clientRequest;
        HttpRequest serverResponse;

        public ProxyserverWindow()
        {
            InitializeComponent();
            settings = new ProxySettingsViewModel {
                Port = 8090, CacheTimeout= 60000, BufferSize=200,
                CheckModifiedContent =true, ContentFilterOn=true,
                BasicAuthOn = false, AllowChangeHeaders= true,
                LogRequestHeaders = false, LogContentIn= true,
                LogContentOut=true, LogCLientInfo = true,
                ServerRunning=false
            };
            // set the binding
            settingsBlock.DataContext = settings;
            logListBox.ItemsSource = LogItems;
            UpdateUIWithLogItem(new HttpRequest(HttpRequest.MESSAGE, settings) {
                LogItemInfo = "Log van:\r\n" +
                "   * request headers\r\n" +
                "   * Response headers\r\n" +
                "   * content in\r\n" +
                "   * content uit\r\n" +
                "   * Client (Which browser is connected)"
            });
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
                UpdateUIWithLogItem(new HttpRequest(HttpRequest.MESSAGE, settings) { LogItemInfo = "Listening for HTTP REQUEST" });
                settings.ServerRunning = true;
                while (true)
                {
                    TcpClient tcpClient = await tcpListner.AcceptTcpClientAsync();
                    NetworkStream clientStream = tcpClient.GetStream();
                    await ListenForHttpRequest(tcpClient);
                    tcpClient.Dispose();
                    clientStream.Dispose();
                }
            }
            
            catch (ObjectDisposedException err)
            {
                UpdateUIWithLogItem(new HttpRequest(HttpRequest.MESSAGE, settings) { LogItemInfo = "Connot get request. server stopped, ERROR LOG: \r\n " + err.Message });
            }
            
            catch (ArgumentException err)
            {
                UpdateUIWithLogItem(new HttpRequest(HttpRequest.MESSAGE, settings) { LogItemInfo = "Argument Exception!: \r\n " + err.Message });
            }
            finally
            {
                StopProxyServer();
            }
        }
        private void UpdateUIWithLogItem(HttpRequest logItem)
        {
           LogItems.Add(logItem);
        }

        private async Task ListenForHttpRequest(TcpClient tcpClient)
        {
            NetworkStream clientStream = tcpClient.GetStream();
                byte[] messageBytes = await streamReader.GetBytesFromReading(settings.BufferSize, clientStream);
                string requestInfo = Encoding.ASCII.GetString(messageBytes, 0, messageBytes.Length);
                // firefox spam requests
                if (!requestInfo.Contains("detectportal"))
                {
                    clientRequest = new HttpRequest(HttpRequest.REQUEST, settings) { LogItemInfo = requestInfo };
                    if (settings.LogContentIn)
                    {
                        UpdateUIWithLogItem(clientRequest);
                    }
                    await HandleHttpRequest(tcpClient);
                }
                
        }
        private async Task HandleHttpRequest(TcpClient tcpClient)
        {
            NetworkStream clientStream = tcpClient.GetStream();
            //check user info if settings say so
            if (settings.BasicAuthOn)
            {
                //jump out if Auth failed
                if (!await DoBasicAuth(clientStream)) return;
            }
            if (cacher.RequestKnown(clientRequest.Method))
            {
                bool contentModified = cacher.ContentModified(cacher.GetKnownResponse(clientRequest.Method), settings.CacheTimeout);
                if (settings.CheckModifiedContent && contentModified)
                {
                    cacher.RemoveItem(clientRequest.Method);
                    await HandleProxyRequest(clientStream);
                    return;
                }
                byte[] knownResponseBytes = cacher.GetKnownResponse(clientRequest.Method).ResponseBytes;
                string knownResponse = Encoding.ASCII.GetString(knownResponseBytes, 0, knownResponseBytes.Length);
                serverResponse = new HttpRequest(HttpRequest.CACHED_RESPONSE, settings) { LogItemInfo = knownResponse };
                if (settings.LogContentOut)
                {
                    UpdateUIWithLogItem(serverResponse);
                }
                await streamReader.WriteMessageWithBufferAsync(clientStream, knownResponseBytes, settings.BufferSize, settings.ContentFilterOn);
                clientStream.Dispose();
                return;
            }
            else
            {
                try
                {
                    await HandleProxyRequest(clientStream);
                    
                }
                catch (UriFormatException err)
                {
                    await SendBadRequest(clientStream);
                    UpdateUIWithLogItem(new HttpRequest(HttpRequest.ERROR, settings) { LogItemInfo = $"Bad request from {clientRequest.Method} ERROR:\r\n {err.Message}" });
                }
                catch (SocketException err)
                {
                    if (err.SocketErrorCode.ToString() == "HostNotFound")
                    {
                        UpdateUIWithLogItem(new HttpRequest(HttpRequest.MESSAGE, settings) { LogItemInfo = "Unable to find host" });
                    }
                    await SendBadRequest(clientStream);
            }
                catch (IOException err)
                {
                    UpdateUIWithLogItem(new HttpRequest(HttpRequest.MESSAGE, settings) { LogItemInfo = "Stream closed: \r\n " + err.Message });
                }
            }
        }
        private async Task HandleProxyRequest(NetworkStream clientStream)
        {
            byte[] responseData = await streamReader.DoProxyRequest(clientRequest);
            string responseString = Encoding.ASCII.GetString(responseData, 0, responseData.Length);
            serverResponse = new HttpRequest(HttpRequest.RESPONSE, settings) { LogItemInfo = responseString };
            //only save non img to cache 
            if (!serverResponse.GetHeader("Content-Type").Contains("image"))
            {
                cacher.addRequest(clientRequest.Method, responseData);
            }
            if (settings.LogContentIn)
            {
                UpdateUIWithLogItem(serverResponse);
            }
            await streamReader.WriteMessageWithBufferAsync(clientStream, responseData, settings.BufferSize, settings.ContentFilterOn);
        }
        private async Task<bool> DoBasicAuth(NetworkStream clientStream)
        {
            string authHeader = clientRequest.GetHeader("Authorization");
            if (authHeader == "")
            {
                await SendUnAutherizedResponse(clientStream);
                return false;
            }
            string encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
            Encoding encoding = Encoding.GetEncoding("iso-8859-1");
            string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));
            if (usernamePassword != "admin:admin")
            {
                await SendUnAutherizedResponse(clientStream);
                return false;
            }
            return true;
        }
        public async Task SendUnAutherizedResponse(NetworkStream clientStream)
        {
            var builder = new StringBuilder();
            builder.AppendLine("HTTP/1.1 401 Unauthorized");
            builder.AppendLine($"Date: {DateTime.Now}");
            builder.AppendLine();
            builder.AppendLine($"<htlm><body><h1>Unauthorized</h1></body></html>");
            builder.AppendLine();
            byte[] badRequestResponse = Encoding.ASCII.GetBytes(builder.ToString());
            await streamReader.WriteMessageWithBufferAsync(clientStream, badRequestResponse, settings.BufferSize);
            UpdateUIWithLogItem(new HttpRequest(HttpRequest.RESPONSE, settings) { LogItemInfo = builder.ToString() });
        }
        public async Task SendBadRequest(NetworkStream clientStream)
        {
            var builder = new StringBuilder();
            builder.AppendLine("HTTP/1.1 400 Bad Request");
            builder.AppendLine($"Date: {DateTime.Now}");
            builder.AppendLine();
            builder.AppendLine("<htlm><body><h1>Bad Request</h1></body></html>");
            builder.AppendLine();
            byte[] badRequestResponse = Encoding.ASCII.GetBytes(builder.ToString());
            await streamReader.WriteMessageWithBufferAsync(clientStream, badRequestResponse, settings.BufferSize);
            UpdateUIWithLogItem(new HttpRequest(HttpRequest.RESPONSE, settings) { LogItemInfo = builder.ToString() });
        }
        private void StopProxyServer()
        {
            if (tcpListner != null)
            UpdateUIWithLogItem(new HttpRequest(HttpRequest.MESSAGE, settings) { LogItemInfo = "Stopping proxy Server..." });
            tcpListner.Stop();
            settings.ServerRunning = false;
            UpdateUIWithLogItem(new HttpRequest(HttpRequest.MESSAGE, settings) { LogItemInfo = "Proxy server Stopped Running" });
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LogItems.Clear();
        }
    }
}
