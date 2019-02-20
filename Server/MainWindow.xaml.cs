using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.IO;
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
            HandleOnSendMessage();
        }
        private void UpdateUI(string message)
        {
            chatBox.Items.Add(message);
        }

        private async void SendToAllClients(string message)
        {
            byte[] bytesToSend = Encoding.ASCII.GetBytes(message);
            foreach (var client in allClients.ToList())
            {
                // check if client is connected else do nothing
                if (client.Key.Connected)
                {
                    await client.Key.GetStream().WriteAsync(bytesToSend, 0, bytesToSend.Length);
                }
            }
        }
        public async void ReceiveData(TcpClient currentClient)
        {
            UpdateUI("Connected!");
            byte[] bytesToSend = Encoding.ASCII.GetBytes($"Verbonden met:{txtServerName.Text}");
            NetworkStream currentStream = currentClient.GetStream();
            currentStream.Write(bytesToSend, 0, bytesToSend.Length);
            try
            {
                // While there is a connection
                while (true)
                {
                    string responseData = await chatApp.GetResonseFromReading(currentStream, Int32.Parse(bufferSize.Text));
                    // Handle client leaving
                    if (responseData == "bye")
                    {
                        string goodbyeMessage = $"Client '{allClients[currentClient]}' has left";
                        UpdateUI(goodbyeMessage);
                        SendToAllClients(goodbyeMessage);
                        allClients.Remove(currentClient);
                        UpdateClientList();
                        bytesToSend = Encoding.ASCII.GetBytes("bye");
                        currentStream.Write(bytesToSend, 0, bytesToSend.Length);
                        return;
                    }
                    // Update all connected clients
                    SendToAllClients($"[{allClients[currentClient]}]: {responseData}");
                    UpdateUI($"[{allClients[currentClient]}]: {responseData}");
                }
            }
            catch (IOException)
            {
                // when user leaves. Remove from the list
                SendToAllClients($"[{allClients[currentClient]}]: Has left the chatroom");
                UpdateUI($"[{allClients[currentClient]}]: Has left the chatroom");
                allClients.Remove(currentClient);
                UpdateClientList();
            }
        }
        private async void StartServer()
        {
            ToggleAllowInput();
            try
            {
                tcpListner = new TcpListener(IPAddress.Any, Int32.Parse(portBox.Text));
                tcpListner.Start();
                UpdateUI("Listening for a client");
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
            catch (FormatException)
            {
                UpdateUI($"Given port adress: {portBox.Text} is not in the correct format (0-9999)");
            }
            catch (SocketException e)
            {
                Console.WriteLine("server error: {0}", e);
                throw e;
            }
            catch (ObjectDisposedException)
            {
                tcpListner.Stop();
                UpdateUI("Server stopped");
            }
            finally
            {
                tcpListner.Stop();
                tcpListner.Stop();
                UpdateUI("Server stopped");
            }


        }

        private void UpdateClientList()
        {
            listClients.Items.Clear();
            if (allClients.Count == 0) 
            {
                listClients.Items.Add("No clients connected yet");
                return;
            }
            foreach (var client in allClients)
            {
                listClients.Items.Add(client.Value);
            }
        }

        private void BtnStartStop_Click(object sender, RoutedEventArgs e)
        {
            UpdateUI("Starting server....");
            StartServer();
        }

        private void ToggleAllowInput()
        {
            bufferSize.IsEnabled = !bufferSize.IsEnabled;
            txtServerName.IsEnabled = !txtServerName.IsEnabled;
            portBox.IsEnabled = !portBox.IsEnabled;
            btnStartStop.IsEnabled = !btnStartStop.IsEnabled;
        }

        private void MsgBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return) HandleOnSendMessage();
        }

        private void HandleOnSendMessage()
        {
            if (msgBox.Text.Length == 0) return;
            SendToAllClients($"[Server]: {msgBox.Text}");
            UpdateUI($"[Server]: {msgBox.Text}");
            msgBox.Clear();
            msgBox.Focus();
        }
    }
}
