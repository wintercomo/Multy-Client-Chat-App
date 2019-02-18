using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

delegate void AddMessageDelegate(string n);
namespace MultyClientChatApp
{
    public partial class MultyChatApp : Form
    {
        private TcpClient tcpClient;
        NetworkStream networkStream;

        // Declare a method with the same signature as the delegate.
        static void Notify(string name)
        {
            Console.WriteLine("Notification received for: {0}", name);
        }

        public MultyChatApp()
        {
            InitializeComponent();
        }

        private void btnSend(object sender, EventArgs e)
        {
            byte[] data = new byte[1024];
            data = System.Text.Encoding.ASCII.GetBytes(msgBox.Text);
            networkStream = tcpClient.GetStream();
            //PROBLEM: networkstream is null
            networkStream.Write(data, 0, data.Length);
            sendMessage(msgBox.Text);

        }
        private void sendMessage(string message)
        {
            
            chatBox.Items.Add(message);
        }
        private void BtnConnect(object sender, EventArgs e)
        {
            try
            {
                sendMessage("Connecting...");
                String server = txtServerIp.Text;
                Int32 port = 9000;
                tcpClient = new TcpClient(server, port);
                // EDIT
                networkStream = tcpClient.GetStream();
                ReceiveData();
                // when connected. disable the button
                btnListen.Enabled = false;
            }
            catch (ArgumentNullException err)
            {
                Console.WriteLine("ArgumentNullException: {0}", err);
            }
            catch (SocketException err)
            {
                Console.WriteLine("SocketException: {0}", err);
            }
        }

        private async void startServer()
        {
            try
            {
                TcpListener tcpListner = new TcpListener(IPAddress.Any, 9000);
                tcpListner.Start();
                sendMessage("Listening for a client");
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
        public void AddMessage(string message)
        {
            chatBox.Items.Add(message);
        }
        public async void ReceiveData()
        {
            int i;
            string s;
            byte[] data = new byte[1024];
            sendMessage("Connected!");
            String responseData = String.Empty;
            data = new Byte[256];
            networkStream = tcpClient.GetStream();
            try
            {
                while (true)
                {
                    Int32 bytes = await networkStream.ReadAsync(data, 0, data.Length);
                    responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                    Console.WriteLine("Received: {0}", responseData);
                    if (responseData == "bye")
                    {
                        break;
                    }
                    sendMessage(responseData);
                }
                data = System.Text.Encoding.ASCII.GetBytes("bye");
                networkStream.Write(data, 0, data.Length);

                networkStream.Close();
                tcpClient.Close();
                sendMessage("Connection closed!");
            }
            catch (Exception err)
            {

                throw err;
            }
        }
       
        private void BtnListen(object sender, EventArgs e)
        {
            sendMessage("Starting server....");
            startServer();
            connectButton.Enabled = false;
        }
    }
}
