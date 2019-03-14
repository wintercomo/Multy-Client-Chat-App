using ClassLibrary1;
using ProxyClasses;
using System;
using System.Collections.Generic;
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
using System.Windows.Media;

namespace ProxyServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ProxyserverWindow : Window
    {
        ObservableCollection<HttpRequest> LogItems = new ObservableCollection<HttpRequest>();
        StreamReader streamReader = new StreamReader();
        Cacher cacher = new Cacher();
        TcpListener tcpListner;
        NetworkStream clientStream;
        TcpClient tcpClient;
        ProxySettings settings;
        HttpRequest clientRequest;
        HttpRequest serverResponse;
        //create a client
                   

        public ProxyserverWindow()
        {
            InitializeComponent();
            settings = new ProxySettings {
                Port = 8090, CacheTimeout= 60000, BufferSize=200,
                CheckModifiedContent =true, ContentFilterOn=true,
                BasicAuthOn = false, AllowChangeHeaders= true,
                LogRequestHeaders = false, LogContentIn= true,
                LogContentOut=true, LogCLientInfo = true,
                ServerRunning=false
            };
            this.color.Background = Brushes.Red;
            // REPLACE THIS WITH PAGE.RECOURSE X:KEY = path
            this.DataContext = settings;
            UpdateUIWithLogItem(new HttpRequest(HttpRequest.MESSAGE, settings) {
                LogItemInfo = "Log van:\r\n" +
                "   * request headers\r\n" +
                "   * Response headers\r\n" +
                "   * content in\r\n" +
                "   * content uit\r\n" +
                "   * Client (Which browser is connected)"
            });
            logListBox.ItemsSource = LogItems;
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
            catch (UriFormatException err)
            {
                UpdateUIWithLogItem(new HttpRequest(HttpRequest.MESSAGE, settings) { LogItemInfo = "Cannot parse host IP: \r\n " + err.Message });
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
                    tcpClient.Dispose();
                    clientStream.Dispose();
                }
            }
        }

        private async Task HandleHttpRequest()
        {
            if (cacher.RequestKnown(clientRequest.Method))
            {
                byte[] knownResponseBytes = cacher.GetKnownResponse(clientRequest.Method);
                string knownResponse = Encoding.ASCII.GetString(knownResponseBytes, 0, knownResponseBytes.Length);
                serverResponse = new HttpRequest(HttpRequest.CACHED_RESPONSE, settings) { LogItemInfo = knownResponse };
                if (settings.LogContentOut)
                {
                    UpdateUIWithLogItem(serverResponse);
                }
                await clientStream.WriteAsync(knownResponseBytes, 0, knownResponseBytes.Length);
                return;
            }
            // Get the actual response
            byte[] responseData = await streamReader.DoProxyRequest(clientRequest, clientStream,  settings.BufferSize);
            string responseString = Encoding.ASCII.GetString(responseData, 0, responseData.Length);
            serverResponse = new HttpRequest(HttpRequest.RESPONSE, settings) { LogItemInfo = responseString };
            if (settings.LogContentIn)
            {
                UpdateUIWithLogItem(serverResponse);
            }
            cacher.addRequest(clientRequest.Method, responseData);
            // if request is known. Send the known response back
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
