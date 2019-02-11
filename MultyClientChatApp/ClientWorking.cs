using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MultyClientChatApp
{
    class ClientWorking
    {
        private Stream ClientStream;
        private TcpClient Client;

        public ClientWorking(TcpClient Client)
        {
            this.Client = Client;
            ClientStream = Client.GetStream();
        }

        public void DoSomethingWithClient()
        {
            Console.WriteLine("doing something");
            StreamWriter sw = new StreamWriter(ClientStream);
            StreamReader sr = new StreamReader(sw.BaseStream);
            sw.WriteLine("Hi. This is x2 TCP/IP easy-to-use server");
            sw.Flush();
            string data;
            try
            {
                while ((data = sr.ReadLine()) != "exit")
                {
                    sw.WriteLine(data);
                    sw.Flush();
                }
            }
            finally
            {
                sw.Close();
            }
        }
    }
}
