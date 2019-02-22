using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Input;

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
        public MainWindow()
        {
            InitializeComponent();
    
        }

        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            HandleSendmessage();
        }
        private void UpdateUI(string message)
        {
            chatBox.Items.Add(message);
        }
        //Send a message to all connected clients
        private async void SendToAllClients(string message)
        {
            byte[] bytesToSend = Encoding.ASCII.GetBytes(message);
            foreach (var client in allClients.ToList())
            {
                // only send message if user is connected
                if (client.Key.Connected)
                {
                    await client.Key.GetStream().WriteAsync(bytesToSend, 0, bytesToSend.Length);
                }
            }
        }
        public async void ReceiveData(TcpClient currentClient)
        {
            UpdateUI("Connected!");
            byte[] bytesToSend = Encoding.ASCII.GetBytes($"Verbonden met:{serverNameBox.Text}");
            NetworkStream currentStream = currentClient.GetStream();
            currentStream.Write(bytesToSend, 0, bytesToSend.Length);
            try
            {
                while (true)
                {
                    string responseData = await chatApp.CreateMessageFromReading(currentStream, Int32.Parse(bufferSize.Text));
                    // handle client leaving
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
            catch (IOException)
            {
                // when user leaves. Remove from the list
                SendToAllClients($"[{allClients[currentClient]}]: Disconnected!");
                UpdateUI($"[{allClients[currentClient]}]: Disconnected!");
                allClients.Remove(currentClient);
                UpdateClientList();
            }
        }
        // Check if port is free
        public static bool PortInUse(int port)

        {
            bool inUse = false;
            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] ipEndPoints = ipProperties.GetActiveTcpListeners();
            foreach (IPEndPoint endPoint in ipEndPoints)
            {
                if (endPoint.Port == port)
                {
                    inUse = true;
                    break;
                }
            }
            return inUse;
        }
        //try starting the server
        private async void StartServer()
        {
            
            try
            {
                ValidateConnectionRequest();
                UpdateUI("Starting server....");
                TcpClient tcpClient;
                tcpListner = new TcpListener(IPAddress.Any, Int32.Parse(portBox.Text));
                tcpListner.Start();
                UpdateUI("Listening for a client");
                ToggleAllowInput();
                while (true)
                {
                    //create a client
                    tcpClient = await tcpListner.AcceptTcpClientAsync();
                    NetworkStream clientStream = tcpClient.GetStream();
                    // get the username
                    string username = await chatApp.CreateMessageFromReading(clientStream, Int32.Parse(bufferSize.Text));
                    //Add the client to the list
                    allClients.Add(tcpClient, username);
                    ReceiveData(tcpClient);
                    UpdateUI($"Total connected clients: {allClients.Count}, New client : {username}");
                    UpdateClientList();
                }
            }
            catch (ArgumentException err)
            {
                UpdateUI(err.Message);
            }
            catch (ObjectDisposedException)
            {
                UpdateUI($"Server is stopped");
            }
            catch (SocketException e)
            {
                
                Console.WriteLine("server error: {0}", e);
            }
            finally
            {
                if (tcpListner != null)
                {
                    tcpListner.Stop();

                }
            }


        }
        // Validate all connection request values
        private void ValidateConnectionRequest()
        {
            if (portBox.Text.Length == 0 || !portBox.Text.All(char.IsDigit)) throw new ArgumentException("Port must be a number");
            if (String.IsNullOrEmpty(serverNameBox.Text)) throw new ArgumentException("Server name cannot empty");
            if (!bufferSize.Text.All(char.IsDigit)) throw new ArgumentException("Buffer size must be a number");
            if (bufferSize.Text.Length > 0 && Int32.Parse(bufferSize.Text) == 0 || String.IsNullOrEmpty(bufferSize.Text)) throw new ArgumentException("Buffer size cannot be 0");
            if (PortInUse(Int32.Parse(portBox.Text))) throw new ArgumentException($"[ERROR] Port: {portBox.Text} is in use");
        }
        // Update the UI with all connected clients
        private void UpdateClientList()
        {
            listClients.Items.Clear();
            foreach (var client in allClients)
            {
                listClients.Items.Add(client.Value);
            }
        }
        // Start server
        private void BtnStartStop_Click(object sender, RoutedEventArgs e)
        {
            
            StartServer();
        }
        //Disables the input fields
        private void ToggleAllowInput()
        {
            bufferSize.IsEnabled = !bufferSize.IsEnabled;
            serverNameBox.IsEnabled = !serverNameBox.IsEnabled;
            portBox.IsEnabled = !portBox.IsEnabled;
            //Not able to stop the server with a button. You are able by closing the form
            btnStartStop.IsEnabled = !btnStartStop.IsEnabled;
        }

        private void MsgBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) HandleSendmessage();
        }

        private void HandleSendmessage()
        {
            if (msgBox.Text.Length == 0) return;
            SendToAllClients($"[Server]: {msgBox.Text}");
            UpdateUI($"[Server]: {msgBox.Text}");
            msgBox.Clear();
            msgBox.Focus();
        }
    }
}
