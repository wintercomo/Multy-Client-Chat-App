﻿using ClassLibrary1;
using ProxyClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
        CacheItem cachedResponse;
        TcpClient tcpClient;
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
        }
        private async void BtnStartStopProxy_Click(object sender, RoutedEventArgs e)
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
            try
            {
                while (true)
                {
                    tcpClient = await tcpListner.AcceptTcpClientAsync();
                    NetworkStream clientStream = tcpClient.GetStream();
                    await ListenForHttpRequest(tcpClient);
                    if (settings.LogContentOut && serverResponse != null) UpdateUIWithLogItem(serverResponse);
                }
            }

            catch (ObjectDisposedException)
            {
                UpdateUIWithLogItem(new HttpRequest(HttpRequest.MESSAGE, settings) { LogItemInfo = "Server not running. Not handling requests: \r\n "});
            }
            catch (ArgumentException err)
            {
                UpdateUIWithLogItem(new HttpRequest(HttpRequest.MESSAGE, settings) { LogItemInfo = "Argument Exception!: \r\n " + err.Message });
            }
            catch (UriFormatException err)
            {
                UpdateUIWithLogItem(new HttpRequest(HttpRequest.ERROR, settings) { LogItemInfo = $"Bad request from {clientRequest.Method} ERROR:\r\n {err.Message}" });
            }
            catch (SocketException)
            {
                UpdateUIWithLogItem(new HttpRequest(HttpRequest.MESSAGE, settings) { LogItemInfo = "Unable to find host" });
            }
            catch (IOException err)
            {
                UpdateUIWithLogItem(new HttpRequest(HttpRequest.MESSAGE, settings) { LogItemInfo = "Stream closed: \r\n " + err.Message });
            }
            catch (BadRequestException err)
            {
                UpdateUIWithLogItem(new HttpRequest(HttpRequest.MESSAGE, settings) { LogItemInfo = "Stream Error. LOG: \r\n " + err.Message });
            }
        }

        private async Task ListenForHttpRequest(TcpClient tcpClient)
        {
            NetworkStream clientStream = tcpClient.GetStream();
            var requestBytes = await streamReader.GetBytesFromReading(settings.BufferSize, clientStream);
            string requestInfo = ASCIIEncoding.ASCII.GetString(requestBytes, 0, requestBytes.Length);
            // firefox spam requests
            if (!requestInfo.Contains("detectportal"))
            {
                clientRequest = new HttpRequest(HttpRequest.REQUEST, settings) { LogItemInfo = requestInfo };
                if (settings.LogContentIn && clientRequest != null) UpdateUIWithLogItem(clientRequest);
                _ = Task.Run(async () => await HandleHttpRequest(tcpClient));
            }
        }
        private async Task HandleHttpRequest(TcpClient tcpClient)
        {
            NetworkStream clientStream = tcpClient.GetStream();
            //check user info if settings say so
            if (settings.BasicAuthOn && !await DoBasicAuth(clientStream)) return; //
            if (cacher.RequestKnown(clientRequest.Method))
            {
                cachedResponse = cacher.GetKnownResponse(clientRequest.Method);
                bool olderThanTimeout = cacher.OlderThanTimeout(cachedResponse, settings.CacheTimeout);
                if (olderThanTimeout)
                {
                    cacher.RemoveItem(clientRequest.Method);
                    await HandleProxyRequest(clientStream);
                    return;
                }
                //CACHE RESPONSE
                HandleCachedResponse();
            }
            await HandleProxyRequest(clientStream);
        }

        private void HandleCachedResponse()
        {
            byte[] knownResponseBytes = cachedResponse.ResponseBytes.ToArray();
            if (settings.ContentFilterOn) knownResponseBytes = streamReader.ReplaceImages(knownResponseBytes);
            string knownResponse = Encoding.ASCII.GetString(knownResponseBytes, 0, knownResponseBytes.Length);
            serverResponse = new HttpRequest(HttpRequest.CACHED_RESPONSE, settings) { LogItemInfo = knownResponse };
            string modifiedDate = serverResponse.GetHeader("Last-Modified");
            clientRequest.UpdateHeader("If-Modified-Since", $" {modifiedDate}");
        }

        private async Task HandleProxyRequest(NetworkStream clientStream)
        {
            byte[] responseData = await streamReader.MakeProxyRequestAsync(clientRequest, settings.BufferSize);
            string responseString = Encoding.ASCII.GetString(responseData, 0, responseData.Length);
            serverResponse = new HttpRequest(HttpRequest.RESPONSE, settings) { LogItemInfo = responseString };
            if (serverResponse.Method.Contains("304 Not Modified"))
            {
                responseData = cachedResponse.ResponseBytes.ToArray();
                serverResponse = new HttpRequest(HttpRequest.CACHED_RESPONSE, settings) { LogItemInfo = responseString };
                await streamReader.WriteMessageWithBufferAsync(clientStream, settings.ContentFilterOn ? streamReader.ReplaceImages(responseData) : responseData, settings.BufferSize);
                return;
            }
            Console.WriteLine($"BEFORE: {ASCIIEncoding.ASCII.GetString(responseData, 0, responseData.Length)}");
            await streamReader.WriteMessageWithBufferAsync(clientStream, settings.ContentFilterOn ? streamReader.ReplaceImages(responseData) : responseData, settings.BufferSize);
            Console.WriteLine($"AFTER: {ASCIIEncoding.ASCII.GetString(responseData, 0, responseData.Length)}");
            await OnEndRequest(clientStream, responseData);
        }

        private async Task OnEndRequest(NetworkStream clientStream, byte[] responseData)
        {
            //Do not save img or partial content
            if (!serverResponse.Method.Contains("Partial Content")
                ) cacher.addRequest(clientRequest.Method, responseData);

            tcpClient.Dispose();
            clientStream.Dispose();
            clientRequest = null;
            serverResponse = null;
            cachedResponse = null;
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
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("HTTP/1.1 401 Unauthorized");
            builder.AppendLine($"Date: {DateTime.Now}");
            builder.AppendLine();
            builder.AppendLine($"<html><body><h1>Unauthorized</h1></body></html>");
            builder.AppendLine();
            byte[] badRequestResponse = Encoding.ASCII.GetBytes(builder.ToString());
            await streamReader.WriteMessageWithBufferAsync(clientStream, badRequestResponse, settings.BufferSize);
            serverResponse = new HttpRequest(HttpRequest.RESPONSE, settings) { LogItemInfo = builder.ToString() };
            tcpClient.Dispose();
            clientStream.Dispose();
            clientRequest = null;
            serverResponse = null;
            cachedResponse = null;
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
        private void UpdateUIWithLogItem(HttpRequest logItem)
        {
           LogItems.Add(logItem);
        }
    }
}
