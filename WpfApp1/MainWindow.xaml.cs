using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TcpClient tcpClient;
        NetworkStream networkStream;
        ChatAppFunctions sharedFunctions = new ChatAppFunctions();
        string ipAdress;
        string portAdress;
        public MainWindow()
        {
            InitializeComponent();
            ipAdress = txtServerIp.Text;
            portAdress = txtPort.Text;
        }
        private void UpdateUI(string message)
        {
            chatBox.Items.Add(message);
        }
        private Boolean validateIP(string ipString)
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
        public async void ReceiveData()
        {
            UpdateUI("Connected!");
            networkStream = tcpClient.GetStream();
            byte[] data = Encoding.ASCII.GetBytes(usernameBox.Text);
            networkStream.Write(data, 0, data.Length);
            data = Encoding.ASCII.GetBytes(msgBox.Text);
            try
            {
                while (true)
                {
                    string responseData = await sharedFunctions.GetResponseData(networkStream, Int32.Parse(txtBufferSize.Text));
                    if (responseData == "bye")
                    {
                        break;
                    }
                    UpdateUI(responseData);
                }
                networkStream.Close();
                tcpClient.Close();
                UpdateUI("Connection closed!");
                ToggleAllowInput();
            }
            catch (SocketException)
            {
                Console.WriteLine("Cannot find a server");
            }
            catch (System.IO.IOException)
            {
                UpdateUI("Host closed connection!");
            }
            catch (Exception err)
            {
                UpdateUI("Oops something went wrong");
                throw err;
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
                String server = txtServerIp.Text;

                // error handling
                if (String.IsNullOrEmpty(usernameBox.Text)) throw new ArgumentException("Username cannot be null or empty");
                if (!validateIP(server)) throw new ArgumentException($"Given IP adress: {server} is not in the correct format");
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
                UpdateUI($"Server on ip: {ipAdress} and port: {portAdress} is not available");
            }
            catch (FormatException)
            {
                UpdateUI($"Given port adress: {portAdress} is not in the correct format (0-9999)");
            }
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            if (tcpClient == null) return;
            byte[] data = new byte[1024];
            data = Encoding.ASCII.GetBytes(msgBox.Text);
            networkStream = tcpClient.GetStream();
            networkStream.Write(data, 0, data.Length);
        }
    }
}
