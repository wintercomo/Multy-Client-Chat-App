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
        public async Task<string> GetResponseData(NetworkStream networkStream, Int32 bufferSize = 1024)
        {
            //TOdO remove this later on
            //Int32 bufferSize = 2;
            StringBuilder responseData = new StringBuilder();

            while (true)
            {
                byte[] buffer = new byte[bufferSize];
                do
                {
                    if (buffer.Length != bufferSize)
                    {
                        buffer = new byte[bufferSize];
                    }
                    int readBytes = await networkStream.ReadAsync(buffer, 0, bufferSize);
                    responseData.AppendFormat("{0}", Encoding.ASCII.GetString(buffer, 0, readBytes));
                } while (networkStream.DataAvailable);
                //Int32 bytes = await networkStream.ReadAsync(bufferSize, 0, bufferSize.Length);
                //responseData = Encoding.ASCII.GetString(bufferSize, 0, bytes);
                if (responseData.ToString() == "bye")
                {
                    break;
                }
                return responseData.ToString();
            }
            return responseData.ToString();
        }
    }
}
