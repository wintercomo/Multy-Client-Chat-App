﻿using System;
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
            StringBuilder responseData = new StringBuilder();
            Console.WriteLine($"RECIEVE MESSAGE: Buffer size : {bufferSize}");
            byte[] buffer = new byte[bufferSize];
            do
            {
                int readBytes = await networkStream.ReadAsync(buffer, 0, bufferSize);
                responseData.AppendFormat("{0}", Encoding.ASCII.GetString(buffer, 0, readBytes));
            } while (networkStream.DataAvailable);
            return responseData.ToString();
        }
    }
}
