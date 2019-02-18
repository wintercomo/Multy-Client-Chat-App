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
        public async Task<string> ReceiveData(NetworkStream networkStream, byte[] data )
        {
            string responseData = "";
            //TOdO remove this later on
            data = new byte[1024];

            while (true)
            {
                Int32 bytes = await networkStream.ReadAsync(data, 0, data.Length);
                responseData = Encoding.ASCII.GetString(data, 0, bytes);
                if (responseData == "bye")
                {
                    break;
                }
                return responseData;
            }
            return responseData;
        }
    }
}
