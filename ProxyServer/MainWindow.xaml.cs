using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
        public MainWindow()
        {
            InitializeComponent();
            logListBox.Items.Add("Log van:\r\n" +
                "   * request headers\r\n" +
                "   * Response headers\r\n" +
                "   * content in\r\n" +
                "   * content uit\r\n" +
                "   * Client (Which browser is connected)");
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
                TcpClient tcpClient;
                tcpListner = new TcpListener(IPAddress.Any, 8090);
                tcpListner.Start();
                UpdateUI("Listening for a client");
                // keep looking for a request
                while (true)
                {
                    //Wait for a client aka a request
                    tcpClient = await tcpListner.AcceptTcpClientAsync();
                    clientStream = tcpClient.GetStream();
                    // get the request details
                    string requestInfo = await chatApp.CreateMessageFromReading(clientStream, 1024);
                    UpdateUI($"NEW Request: \r\n {requestInfo}");
                    // TODO
                    // get the actual response and return it to the client
                    byte[] bytesToSend = Encoding.ASCII.GetBytes("CONNECTEDDDDDDDDDDDDDDDDDDDDDD");
                    await clientStream.WriteAsync(bytesToSend, 0, bytesToSend.Length);
                    clientStream.Close();
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
        private void UpdateUI(String logMessage)
        {
            logListBox.Items.Add(logMessage);
        }

        private void StopProxyServer()
        {
            UpdateUI("Stopping proxy Server...");
            tcpListner.Stop();
            UpdateUI("Server stopped!");
        }
    }
}
