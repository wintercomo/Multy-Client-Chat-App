using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class ChatAppFunctions
    {
        // Collects the whole message
        // Returns the collected message
        public async Task<string> GetResonseFromReading(NetworkStream networkStream, Int32 bufferSize = 1024)
        {
            StringBuilder responseData = new StringBuilder();
            byte[] buffer = new byte[bufferSize];
            do
            {
                int readBytes = await networkStream.ReadAsync(buffer, 0, bufferSize);
                responseData.AppendFormat($"{Encoding.ASCII.GetString(buffer, 0, readBytes)}");
            } while (networkStream.DataAvailable);
            return responseData.ToString();
        }
    }
}
