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
        private Thread thread;
        List<string> _items = new List<string>();
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
            _items.Add("One");
            chatBox.DataSource = _items;
        }

        private void btnSend(object sender, EventArgs e)
        {
            byte[] data = new byte[1024];
            data = System.Text.Encoding.ASCII.GetBytes(msgBox.Text);
            networkStream = tcpClient.GetStream();
            //PROBLEM: networkstream is null
            _items.Add(networkStream.ToString());
            chatBox.DataSource = null;
            chatBox.DataSource = _items;
            networkStream.Write(data, 0, data.Length);
            sendMessage(msgBox.Text);

        }
        private void sendMessage(string message)
        {
            if (this.chatBox.InvokeRequired)
            {
                AddMessageDelegate addMessage = new AddMessageDelegate(AddMessage);
                // this is the delegate
                this.Invoke(addMessage, _items, message);
            }
            else
            {
                _items.Add(message);
                chatBox.DataSource = null;
                chatBox.DataSource = _items;
            }

        }
        private void BtnConnect(object sender, EventArgs e)
        {
            try
            {
                _items.Add("connecting...");
                chatBox.DataSource = null;
                chatBox.DataSource = _items;
                Console.WriteLine("Trying to connect");
                String server = "127.0.0.1";
                Int32 port = 9000;
                tcpClient = new TcpClient(server, port);
                // EDIT
                networkStream = tcpClient.GetStream();
                thread = new Thread(new ThreadStart(ReceiveData));
                thread.Start();

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

        private void startServer()
        {
            try
            {
               
                Int32 port = 9000;
                IPAddress localAddr = IPAddress.Any;
                TcpListener tcpListner = new TcpListener(localAddr, port);
                tcpListner.Start();
                this._items.Add("Listening for a client");
                this.chatBox.DataSource = null;
                this.chatBox.DataSource = _items;
                while (true)
                {
                    tcpClient = tcpListner.AcceptTcpClient();
                    thread = new Thread(new ThreadStart(ReceiveData));
                    thread.Start();
                }
                Console.WriteLine("Stopping server");
                tcpListner.Stop();


            }
            catch (SocketException e)
            {
                Console.WriteLine("server error: {0}", e);
            }


        }
        public void AddMessage(string message)
        {
            _items.Add(message);
            chatBox.DataSource = null;
            chatBox.DataSource = _items;
        }
        public void ReceiveData()
        {
            try
            {
            int i;
            string s;
            byte[] data = new byte[1024];
            AddMessageDelegate newMessage = new AddMessageDelegate(AddMessage);
            newMessage("Connected!");
            while (true)
            {
                String responseData = String.Empty;
                data = new Byte[256];
                networkStream = tcpClient.GetStream();
                Int32 bytes = networkStream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine("Received: {0}", responseData);
                if (responseData == "bye")
                {
                    break;
                }
                newMessage(responseData);
            }
                data = System.Text.Encoding.ASCII.GetBytes("bye");
                networkStream.Write(data, 0, data.Length);

                //networkStream.Close();
                //tcpClient.Close();
                newMessage("Connection closed!");
            }
            catch (Exception err)
            {

                throw err;
            }
        }
        public static void ThreadCounter()
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("Current Thread : {0}",
                Thread.CurrentThread.Name);
                Console.WriteLine(i);
                Thread.Sleep(1000);
            }

        }
        private void BtnListen(object sender, EventArgs e)
        {
            _items.Add("Starting server....");
            chatBox.DataSource = null;
            chatBox.DataSource = _items;
            thread = new Thread(new ThreadStart(startServer));
            thread.Start();
            //startServer();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }



        private static void delegateMethodInputString(String name)
        {

        }



        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
