using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace MultyClientChatApp
{
    public partial class MultyChatApp : Form
    {
        private Thread thread;
        List<string> _items = new List<string>();
        private TcpClient tcpClient;
        NetworkStream networkStream;
        public delegate void myDelegate(string s);
        delegate void Del(string str);

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

            _items.Add(msgBox.Text);
            chatBox.DataSource = null;
            chatBox.DataSource = _items;
            //Console.WriteLine("Send message!");
            //chatBox.Text = $"{chatBox.Text}\r\n{msgBox.Text}";

        }
        private void checkInvoked()
        {
            if (this.chatBox.InvokeRequired)
            {

                Del del1 = new Del(Notify);
                this.Invoke(del1);
            }
            
        }
        private void BtnConnect(object sender, EventArgs e)
        {
            try
            {
                Console.WriteLine("Trying to connect");
                String server = "127.0.0.1";
                Int32 port = 9000;
                tcpClient = new TcpClient(server, port);
                _items.Add("connecting...");
                chatBox.DataSource = null;
                chatBox.DataSource = _items;

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
                Console.WriteLine("Starting server");
                Int32 port = 9000;
                IPAddress localAddr = IPAddress.Any;
                TcpListener tcpListner = new TcpListener(localAddr, port);
                tcpListner.Start();
                this._items.Add("Listening for a client");
                this.chatBox.DataSource = null;
                this.chatBox.DataSource = _items;
                Console.WriteLine("Server started");
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
        public void ReceiveData()
        {
            int i;
            string s;
            byte[] byteArr = new byte[1024];
            Console.WriteLine("sending data");
            networkStream = tcpClient.GetStream();
            _items.Add("Connected!");
            chatBox.DataSource = null;
            chatBox.DataSource = _items;
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
            _items.Add("Trying to connect ....");
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
