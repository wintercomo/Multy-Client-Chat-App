using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

delegate void AddMessageDelegate(string n);
namespace MultyClientChatApp
{
    public partial class MultyChatApp : Form
    {
        TcpClient tcpClient;
        NetworkStream networkStream;
        ChatAppFunctions sharedFunctions = new ChatAppFunctions();
        string ipAdress;
        string portAdress;


        public MultyChatApp()
        {
            InitializeComponent();
            ipAdress = txtServerIp.Text;
            portAdress = portBox.Text;
        }

        private void BtnSend(object sender, EventArgs e)
        {
            if (tcpClient == null) return;
            byte[] data = new byte[1024];
            data = Encoding.ASCII.GetBytes(msgBox.Text);
            networkStream = tcpClient.GetStream();
            networkStream.Write(data, 0, data.Length);
        }
        private void UpdateUI(string message)
        {
            chatBox.Items.Add(message);
        }
        private async void BtnConnect(object sender, EventArgs e)
        {
            try
            {
                UpdateUI("Connecting...");
                if (String.IsNullOrEmpty(usernameBox.Text))
                {
                    throw new ArgumentException("Username cannot be null or empty");
                }
                if (!validateIP(txtServerIp.Text))
                {
                    throw new ArgumentException($"Given IP adress: {txtServerIp.Text} is not in the correct format");
                }
                String server = txtServerIp.Text;
                Int32 port = Int32.Parse(portBox.Text);
                tcpClient = new TcpClient();
                await tcpClient.ConnectAsync(server, port);
                ReceiveData();
                portBox.Enabled = false;
                txtServerIp.Enabled = false;
                connectButton.Enabled = false;
            }
            catch (ArgumentException err)
            {
                UpdateUI(err.Message);
            }
            catch (SocketException)
            {
                UpdateUI($"Server on ip: {ipAdress} and port: {portAdress} is not available" );
            }
            catch (FormatException)
            {
                UpdateUI($"Given port adress: {portAdress} is not in the correct format (0-9999)" );
            }
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
                    string responseData = await sharedFunctions.GetResponseData(networkStream, data);
                    if (responseData == "bye")
                    {
                        break;
                    }
                    UpdateUI(responseData);
                }
                networkStream.Close();
                tcpClient.Close();
                UpdateUI("Connection closed!");
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
        
        private void chatBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void txtServerIp_TextChanged(object sender, EventArgs e)
        {
            ipAdress = txtServerIp.Text;
        }

        private void portBox_TextChanged(object sender, EventArgs e)
        {
            portAdress = portBox.Text;
        }
    }
}
//public async void ReceiveData()
//{
//    int i;
//    string s;
//    byte[] data = new byte[1024];
//    sendMessage("Connected!");
//    String responseData = String.Empty;
//    data = new Byte[256];
//    networkStream = tcpClient.GetStream();
//    try
//    {
//        while (true)
//        {
//            Int32 bytes = await networkStream.ReadAsync(data, 0, data.Length);
//            responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
//            Console.WriteLine("Received: {0}", responseData);
//            if (responseData == "bye")
//            {
//                break;
//            }
//            sendMessage(responseData);
//        }
//        data = System.Text.Encoding.ASCII.GetBytes("bye");
//        networkStream.Write(data, 0, data.Length);

//        networkStream.Close();
//        tcpClient.Close();
//        sendMessage("Connection closed!");
//    }
//    catch (Exception err)
//    {

//        throw err;
//    }
//}