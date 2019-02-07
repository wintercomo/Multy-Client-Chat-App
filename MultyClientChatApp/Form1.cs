using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultyClientChatApp
{
    public partial class MultyChatApp : Form
    {
        private bool networkStreamOwnsSocket;
        private Socket mySocket;
        private Thread clientThread;
        private int threadCounter = 0;

        public MultyChatApp()
        {
            InitializeComponent();
        }

        private void btnSend(object sender, EventArgs e)
        {
            Console.WriteLine("Send message!");
            chatBox.Text = $"{chatBox.Text}\r\n{msgBox.Text}";

        }

        private void button2_Click(object sender, EventArgs e)
        {

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

        private void button3_Click(object sender, EventArgs e)
        {
            clientThread = new Thread(new ThreadStart(ThreadCounter));
            clientThread.Name = txtServerIp.Text; // threadCounter.ToString();
            threadCounter++;
            clientThread.Start();
        }

        private static void delegateMethodInputString(String name)
        {

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

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
