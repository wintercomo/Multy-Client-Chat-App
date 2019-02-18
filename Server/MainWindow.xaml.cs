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

namespace Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TcpClient tcpClient;
        NetworkStream networkStream;
        ChatApp sharedFunctions = new ChatApp();
        TcpListener tcpListner;
        private string serverName;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            byte[] data = new byte[1024];
            data = Encoding.ASCII.GetBytes(msgBox.Text);
            networkStream = tcpClient.GetStream();
            networkStream.Write(data, 0, data.Length);
            SendMessage(msgBox.Text);
        }
        private void SendMessage(string message)
        {
            chatBox.Items.Add(message);
        }
        public async void ReceiveData()
        {
            SendMessage("Connected!");
            networkStream = tcpClient.GetStream();
            byte[] data = Encoding.ASCII.GetBytes($"Verbonden met:{serverName}");
            networkStream.Write(data, 0, data.Length);
            data = Encoding.ASCII.GetBytes(bufferSize.Text);
            try
            {
                while (true)
                {
                    string responseData = await sharedFunctions.ReceiveData(networkStream, data);
                    if (responseData == "bye")
                    {
                        break;
                    }
                    SendMessage(responseData);
                }
                networkStream.Close();
                tcpClient.Close();
                SendMessage("Connection closed!");
            }
            catch (Exception err)
            {
                throw err;
            }
        }
        private async void StartServer()
        {
            if (tcpListner != null)
            {
                SendMessage("Server is already running");
                return;
            }
            try
            {
                int port = Int32.Parse(portBox.Text);
                tcpListner = new TcpListener(IPAddress.Any, port);
                tcpListner.Start();
                SendMessage("Listening for a client");
                while (true)
                {
                    tcpClient = await tcpListner.AcceptTcpClientAsync();
                    ReceiveData();
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("server error: {0}", e);
            }


        }
        private void BtnStartStop_Click(object sender, RoutedEventArgs e)
        {
            SendMessage("Starting server....");
            StartServer();
        }

        private void ChatBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            serverName = serverNameBox.Text;
        }
    }
}
