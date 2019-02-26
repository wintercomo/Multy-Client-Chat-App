using ClassLibrary1;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace MultyClientChatClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window
    {
        TcpClient tcpClient;
        NetworkStream networkStream;
        ChatAppFunctions sharedFunctions = new ChatAppFunctions();
        public ClientWindow()
        {
            InitializeComponent();
        }
        private void UpdateUI(string message)
        {
            chatBox.Items.Add(message);
        }
        //Validate the given IP Adres
        private Boolean ValidateIpAdress(string ipString)
        {
            if (String.IsNullOrWhiteSpace(ipString)) return false;
            if (!Regex.IsMatch(ipString, @"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b")) return false;
            string[] splitValues = ipString.Split('.');
            if (splitValues.Length != 4) return false;
            return true;
        }
        // Handle recieving data
        public async void ReceiveData()
        {
            UpdateUI("Connected!");
            networkStream = tcpClient.GetStream();
            byte[] data = Encoding.ASCII.GetBytes(usernameBox.Text);
            networkStream.Write(data, 0, data.Length);
            try
            {
                while (true)
                {
                    string responseData = await sharedFunctions.CreateMessageFromReading(networkStream, Int32.Parse(txtBufferSize.Text));
                    if (responseData == "bye")
                    {
                        break;
                    }
                    UpdateUI(responseData);
                }
                networkStream.Close();
                tcpClient.Close();
                UpdateUI("Host closed connection!");
                ToggleAllowInput();
            }
            catch (SocketException)
            {
                UpdateUI("Cannot find a server");
            }
            catch (System.IO.IOException)
            {
                UpdateUI("Host closed connection!");
                ToggleAllowInput();
            }
            catch (Exception err)
            {
                UpdateUI("Oops something went wrong");
                throw err;
            }
            finally
            {
                networkStream.Close();
                tcpClient.Close();
                AllowInput();
            }
        }
        private void AllowInput()
        {
            txtPort.IsEnabled = true;
            txtServerIp.IsEnabled = true;
            connectButton.IsEnabled = true;
            usernameBox.IsEnabled = true;
            txtBufferSize.IsEnabled = true;
        }
        //Disable all connection inputs
        private void ToggleAllowInput()
        {
            txtPort.IsEnabled = !txtPort.IsEnabled;
            txtServerIp.IsEnabled = !txtServerIp.IsEnabled;
            connectButton.IsEnabled = !connectButton.IsEnabled;
            usernameBox.IsEnabled = !usernameBox.IsEnabled;
            txtBufferSize.IsEnabled = !txtBufferSize.IsEnabled;
        }
        //Try connecting to a server
        private async void BtnConnect(object sender, RoutedEventArgs e)
        {
            try
            {
                // error handling
                String server = txtServerIp.Text;
                ValidateConnectionRequest(server);
                UpdateUI("Connecting...");
                //PASS => create a client
                Int32 port = Int32.Parse(txtPort.Text);
                tcpClient = new TcpClient();
                await tcpClient.ConnectAsync(server, port);
                ReceiveData();
                //Disable buttons and input fields
                ToggleAllowInput();

            }
            catch (ArgumentException err)
            {
                UpdateUI(err.Message);
            }
            catch (SocketException)
            {
                UpdateUI($"Server on ip: '{txtServerIp.Text}' and port: '{txtPort.Text}' is not available");
            }
            catch (FormatException err)
            {
                UpdateUI($"Given port adress: '{txtPort.Text}' is not in the correct format (0-9999)");
            }
        }

        private void ValidateConnectionRequest(string server)
        {
            if (!txtPort.Text.All(char.IsDigit)) throw new ArgumentException("Port must be a number");
            if (String.IsNullOrEmpty(usernameBox.Text)) throw new ArgumentException("Username cannot be null or empty");
            if (!txtBufferSize.Text.All(char.IsDigit)) throw new ArgumentException("Buffer size must be a number");
            if (txtBufferSize.Text.Length > 0 && Int32.Parse(txtBufferSize.Text) == 0 || String.IsNullOrEmpty(txtBufferSize.Text)) throw new ArgumentException("Buffer size cannot be 0");
            if (!ValidateIpAdress(server)) throw new ArgumentException($"Given IP adress: '{server}' is not in the correct format");
        }

        // try sending a message
        private void HandleSendmessage()
        {
            try
            {
                byte[] data = Encoding.ASCII.GetBytes(msgBox.Text);
                networkStream = tcpClient.GetStream();
                networkStream.Write(data, 0, data.Length);
            }
            catch (NullReferenceException)
            {
                UpdateUI($"Message {msgBox.Text} cannot be send.\r\n" +
                    $"[REASON]: Client is not connected to any server");
            }
            catch (InvalidOperationException)
            {
                UpdateUI($"Message {msgBox.Text} cannot be send.\r\n" +
                    $"[REASON]: Client is not connected to any server");
            }
            msgBox.Clear();
            msgBox.Focus();
        }
        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            HandleSendmessage();
        }
        private void MsgBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) HandleSendmessage();
        }
    }
}
