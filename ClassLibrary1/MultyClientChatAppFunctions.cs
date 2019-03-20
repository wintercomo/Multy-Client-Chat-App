using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class ChatAppFunctions
    {
        public async Task<string> CreateMessageFromReading(NetworkStream networkStream, Int32 bufferSize = 1024)
        {
            StringBuilder responseData = new StringBuilder();
            byte[] buffer = new byte[bufferSize];
            do
            {
                int readBytes = await networkStream.ReadAsync(buffer, 0, buffer.Length);
                responseData.AppendFormat("{0}", ASCIIEncoding.ASCII.GetString(buffer, 0, readBytes));
            } while (networkStream.DataAvailable);
            return responseData.ToString();
        }

        public async Task<int> getMessageBytes(NetworkStream networkStream, Int32 bufferSize = 1024)
        {
            byte[] buffer = new byte[bufferSize];
            int readBytes = 0;
            do
            {
                 readBytes = await networkStream.ReadAsync(buffer, 0, bufferSize);

            } while (networkStream.DataAvailable);
            return readBytes;
        }
    }
}
