﻿using System;
using System.IO;
using System.IO.Compression;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ProxyClasses
{
    public class StreamReader
    {
        public static string getBetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                return "";
            }
        }
        public async Task<byte[]> DoProxyRequest(HttpRequest httpRequest,NetworkStream clientStream, Int32 bufferSize = 1024)
        {
            string httpRequestString = httpRequest.HttpString;
            string hostString = httpRequest.GetHeader("Host");
            Uri baseUri = new Uri("http://" + hostString);
            TcpClient tcp = new TcpClient(baseUri.Host, baseUri.Port);
            var stream = tcp.GetStream();
            var header = Encoding.ASCII.GetBytes(httpRequestString);
            await stream.WriteAsync(header, 0, header.Length);
            //this freezes the UI. NEED FIX!
            byte[] buffer = new byte[bufferSize];
            MemoryStream memory = new MemoryStream();
            do
            {
                int readBytes = await stream.ReadAsync(buffer, 0, buffer.Length);
                await clientStream.WriteAsync(buffer, 0, readBytes);
                await memory.WriteAsync(buffer, 0, readBytes);
            } while (stream.DataAvailable);
                //await stream.CopyToAsync(memory);
                stream.Dispose();
                tcp.Dispose();
            return memory.ToArray();
        }

        public async Task SendBytesToStream(byte[] message, NetworkStream stream)
        {
            await stream.WriteAsync(message,0, message.Length);
        }
        public async Task<byte[]> GetBytesFromReading(int bufferSize, NetworkStream stream)
        {
            byte[] buffer = new byte[bufferSize];
            var memory = new MemoryStream();
            do
            {
                int readBytes = await stream.ReadAsync(buffer, 0, buffer.Length);
                await memory.WriteAsync(buffer, 0, readBytes);
            } while (stream.DataAvailable);
            return memory.ToArray();
        }
    }
}
