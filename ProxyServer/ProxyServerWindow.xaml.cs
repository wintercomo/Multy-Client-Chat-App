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
        readonly StreamReader streamReader = new StreamReader();
        Cacher cacher = new Cacher();
        TcpListener tcpListner;
        NetworkStream clientStream;
        TcpClient tcpClient;
        ProxySettingsViewModel settings;
        HttpRequest clientRequest;
        HttpRequest serverResponse;
        //create a client
                   

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
        private void UpdateUIWithLogItem(HttpRequest logItem)
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
                await ListenForHttpRequest();
                return;
            }
            catch (HttpListenerException)
            {
                UpdateUIWithLogItem(new HttpRequest(HttpRequest.MESSAGE, settings) { LogItemInfo = "Server stopped running Due to ERROR" });
            }
            catch (ObjectDisposedException err)
            {
                UpdateUIWithLogItem(new HttpRequest(HttpRequest.MESSAGE, settings) { LogItemInfo = "Connot get request. server stopped, ERROR LOG: \r\n " + err.Message });
            }
            
            catch (ArgumentException err)
            {
                UpdateUIWithLogItem(new HttpRequest(HttpRequest.MESSAGE, settings) { LogItemInfo = "Argument Exception!: \r\n " + err.Message });
            }
            catch (IOException err)
            {
                UpdateUIWithLogItem(new HttpRequest(HttpRequest.MESSAGE, settings) { LogItemInfo = "Refused connection! Error log: \r\n " + err.Message });
            }
            finally
            {
                StopProxyServer();
            }
        }

        private async Task ListenForHttpRequest()
        {
            tcpListner = new TcpListener(IPAddress.Any, settings.Port);
            tcpListner.Start();
            UpdateUIWithLogItem(new HttpRequest(HttpRequest.MESSAGE, settings) { LogItemInfo = "Listening for HTTP REQUEST" });
            settings.ServerRunning = true;
            while (true)
            {
                tcpClient = await tcpListner.AcceptTcpClientAsync();
                clientStream = tcpClient.GetStream();
                byte[] messageBytes = await streamReader.GetBytesFromReading(settings.BufferSize, clientStream);
                Console.WriteLine($"MESSAGE BUFF {messageBytes.Length}");

                string requestInfo = Encoding.ASCII.GetString(messageBytes, 0, messageBytes.Length);
                // firefox spam requests
                if (!requestInfo.Contains("detectportal"))
                {
                    clientRequest = new HttpRequest(HttpRequest.REQUEST, settings) { LogItemInfo = requestInfo };
                    if (settings.LogContentIn)
                    {
                        UpdateUIWithLogItem(clientRequest);
                    }
                    await HandleHttpRequest();
                }
                clientStream.Dispose();
                tcpClient.Dispose();
            }
        }
        private async Task<bool> DoBasicAuth()
        {
            string authHeader = clientRequest.GetHeader("Authorization");
            if (authHeader == "")
            {
                await SendUnAutherizedResponse();
                return false;
            }
            string encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
            Encoding encoding = Encoding.GetEncoding("iso-8859-1");
            string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));
            Console.WriteLine(usernamePassword);
            if (usernamePassword != "admin:admin")
            {
                await SendUnAutherizedResponse();
                return false;
            }
            return true;
        }
        private async Task HandleHttpRequest()
        {
            //check user info if settings say so
            if (settings.BasicAuthOn)
            {
                //jump out if Auth failed
                if (!await DoBasicAuth()) return;
            }
            if (cacher.RequestKnown(clientRequest.Method))
            {
                bool contentModified = cacher.ContentModified(cacher.GetKnownResponse(clientRequest.Method), settings.CacheTimeout);
                if (settings.CheckModifiedContent && contentModified)
                {
                    cacher.RemoveItem(clientRequest.Method);
                    await HandleProxyRequest();
                    return;
                }
                byte[] knownResponseBytes = cacher.GetKnownResponse(clientRequest.Method).RequestBytes;
                await streamReader.WriteMessageWithBufferAsync(clientStream, knownResponseBytes, settings.BufferSize, settings.ContentFilterOn);
                //await clientStream.WriteAsync(knownResponseBytes, 0, knownResponseBytes.Length);
                string knownResponse = Encoding.ASCII.GetString(knownResponseBytes, 0, knownResponseBytes.Length);
                serverResponse = new HttpRequest(HttpRequest.CACHED_RESPONSE, settings) { LogItemInfo = knownResponse };
                if (settings.LogContentOut)
                {
                    UpdateUIWithLogItem(serverResponse);
                }
                return;
            }
            // Get the actual response
            try
            {
                await HandleProxyRequest();
            }
            catch (BadRequestException err)
            {
                await SendBadRequest();
                UpdateUIWithLogItem(new HttpRequest(HttpRequest.ERROR, settings) { LogItemInfo = $"Bad request from {clientRequest.Method} ERROR:\r\n {err.Message}" });
            }
            catch (UriFormatException err)
            {
                UpdateUIWithLogItem(new HttpRequest(HttpRequest.MESSAGE, settings) { LogItemInfo = "Cannot parse host IP: \r\n " + err.Message });
            }
        }

        public async Task SendUnAutherizedResponse()
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
        public async Task SendBadRequest()
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

        

        private async Task HandleProxyRequest()
        {
            byte[] responseData = await streamReader.DoProxyRequest(clientRequest, clientStream, settings.BufferSize);

            await streamReader.WriteMessageWithBufferAsync(clientStream, responseData, settings.BufferSize, settings.ContentFilterOn);
            string responseString = Encoding.ASCII.GetString(responseData, 0, responseData.Length);
            serverResponse = new HttpRequest(HttpRequest.RESPONSE, settings) { LogItemInfo = responseString };
            if (settings.LogContentIn)
            {
                UpdateUIWithLogItem(serverResponse);
            }
            cacher.addRequest(clientRequest.Method, responseData);
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
