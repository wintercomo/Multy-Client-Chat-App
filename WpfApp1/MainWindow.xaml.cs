using ClassLibrary1;
using System;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace MultyClientChatClient
{
    /// <summary>
    /// WPF of a client that can connect to a tcp listener
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
        //Checks if an given IP adress is a correct IP adress
        private Boolean ValidateIpAdress(string ipString)
        {
            if (String.IsNullOrWhiteSpace(ipString))
            {
                return false;
            }

            string[] splitValues = ipString.Split('.');
            if (splitValues.Length != 4)
            {
                return false;
            }
            return true;
        }
        // Handle a recieved message
        public async void ReceiveData()
        {
            networkStream = tcpClient.GetStream();
            byte[] data = Encoding.ASCII.GetBytes(usernameBox.Text);
            networkStream.Write(data, 0, data.Length);
            data = Encoding.ASCII.GetBytes(msgBox.Text);
            try
            {
                while (true)
                {
                    string responseData = await sharedFunctions.GetResonseFromReading(networkStream, Int32.Parse(txtBufferSize.Text));
                    if (responseData == "bye")
                    {
                        break;
                    }
                    UpdateUI(responseData);
                }
                networkStream.Close();
                tcpClient.Close();
                UpdateUI("You have left the chat.");
                ToggleAllowInput();
            }
            catch (SocketException)
            {
                Console.WriteLine("Cannot find a server");
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
            }
        }
        private void ToggleAllowInput()
        {
            txtPort.IsEnabled = !txtPort.IsEnabled;
            txtServerIp.IsEnabled = !txtServerIp.IsEnabled;
            connectButton.IsEnabled = !connectButton.IsEnabled;
            usernameBox.IsEnabled = !usernameBox.IsEnabled;
            txtBufferSize.IsEnabled = !txtBufferSize.IsEnabled;
        }
        private async void BtnConnect(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateUI("Connecting...");
                // error handling
                if (String.IsNullOrEmpty(usernameBox.Text)) throw new ArgumentException("Username cannot be null or empty");
                if (!ValidateIpAdress(txtServerIp.Text)) throw new ArgumentException($"Given IP adress: {txtServerIp.Text} is not in the correct format");
                //PASS => create a client
                Int32 port = Int32.Parse(txtPort.Text);
                tcpClient = new TcpClient();
                await tcpClient.ConnectAsync(txtServerIp.Text, port);
                ReceiveData();
                ToggleAllowInput();
            }
            catch (ArgumentException err)
            {
                UpdateUI(err.Message);
            }
            catch (SocketException)
            {
                UpdateUI($"Server on ip: {txtServerIp.Text} and port: {txtPort.Text} is not available");
            }
            catch (FormatException)
            {
                UpdateUI($"Given port adress: {txtPort.Text} is not in the correct format (0-9999)");
            }
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            HandleSendMessage();
        }

        private void HandleSendMessage()
        {
            try
            {
                byte[] data = Encoding.ASCII.GetBytes(msgBox.Text);
                networkStream = tcpClient.GetStream();
                networkStream.Write(data, 0, data.Length);
                msgBox.Focus();
                msgBox.Clear();
            }
            catch (NullReferenceException)
            {
                UpdateUI($"Message {msgBox.Text} cannot be send.\r\n" +
                    $"[REASON]: Client is not connected to any server");
            }
            catch (InvalidOperationException err)
            {
                UpdateUI($"Message {msgBox.Text} cannot be send.\r\n" +
                    $"[REASON]: Client is not connected to any server");
            }
        }

        private void MsgBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (msgBox.Text.Length == 0) return;
            if (e.Key == Key.Return)
            {
                HandleSendMessage();
            }
        }
    }
}
