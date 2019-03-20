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
        ChatAppFunctions chatApp = new ChatAppFunctions();
        TcpListener tcpListner;
        private string serverName;
        Dictionary<TcpClient, string> allClients = new Dictionary<TcpClient, string>();

        private Dictionary<TcpClient, string> GetClients()
        {
            return allClients;
        }
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            SendToAllClients($"[Server]: {msgBox.Text}");
            UpdateUI($"[Server]: {msgBox.Text}");
        }

        private void UpdateUI(string message)
        {
            chatBox.Items.Add(message);
        }

        private async void SendToAllClients(string message)
        {
            byte[] bytesToSend = Encoding.ASCII.GetBytes(message);
            foreach (var client in allClients)
            {
                await client.Key.GetStream().WriteAsync(bytesToSend, 0, bytesToSend.Length);
            }
        }
        public async void ReceiveData(TcpClient currentClient)
        {
            UpdateUI("Connected!");
            byte[] bytesToSend = Encoding.ASCII.GetBytes($"Verbonden met:{serverName}");
            NetworkStream currentStream = currentClient.GetStream();
            currentStream.Write(bytesToSend, 0, bytesToSend.Length);
            try
            {
                while (true)
                {
                    bytesToSend = Encoding.ASCII.GetBytes(bufferSize.Text);
                    string responseData = await chatApp.GetResponseData(currentStream, Int32.Parse(bufferSize.Text));
                    if (responseData == "bye")
                    {
                        string goodbyeMessage = $"Client '{allClients[currentClient]}' has left";
                        UpdateUI(goodbyeMessage);
                        allClients.Remove(currentClient);
                        UpdateClientList();
                        SendToAllClients(goodbyeMessage);
                        bytesToSend = Encoding.ASCII.GetBytes("bye");
                        currentStream.Write(bytesToSend, 0, bytesToSend.Length);
                        return;
                    }
                    // Update all connected clients
                    SendToAllClients($"[{allClients[currentClient]}]: {responseData}");
                    UpdateUI($"[{allClients[currentClient]}]: {responseData}");
                }
            }
            catch (Exception err)
            {
                // when user leaves. Remove from the list
                throw err;
            }
        }
        private async void StartStopServer()
        {
            if (tcpListner != null)
            {
                UpdateUI("Stopping server...");
                tcpListner.Stop();
                ToggleAllowInput();
                return;
            }
            ToggleAllowInput();
            try
            {
                int port = Int32.Parse(portBox.Text);
                tcpListner = new TcpListener(IPAddress.Any, port);
                tcpListner.Start();
                UpdateUI("Listening FUCKKKKK a client");
                while (true)
                {
                    //create a client
                    TcpClient tcpClient = await tcpListner.AcceptTcpClientAsync();
                    NetworkStream clientStream = tcpClient.GetStream();
                    // get the username
                    byte[] data = new byte[1024];
                    Int32 bytes = await clientStream.ReadAsync(data, 0, data.Length);
                    string username = Encoding.ASCII.GetString(data, 0, bytes);
                    //Add the client to the list
                    allClients.Add(tcpClient, username);
                    ReceiveData(tcpClient);
                    UpdateUI($"Total connected clients{allClients.Count}, New client : {username}");
                    UpdateClientList();
                }

            }
            catch (SocketException e)
            {
                
                Console.WriteLine("server error: {0}", e);
            }


        }

        private void UpdateClientList()
        {
            listClients.Items.Clear();
            foreach (var client in allClients)
            {
                listClients.Items.Add(client.Value);
            }
        }

        private void BtnStartStop_Click(object sender, RoutedEventArgs e)
        {
            UpdateUI("Starting server....");
            StartStopServer();
        }

        private void ToggleAllowInput()
        {
            bufferSize.IsEnabled = !bufferSize.IsEnabled;
            serverNameBox.IsEnabled = !serverNameBox.IsEnabled;
            portBox.IsEnabled = !portBox.IsEnabled;
            btnStartStop.Text = "Stop";
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
