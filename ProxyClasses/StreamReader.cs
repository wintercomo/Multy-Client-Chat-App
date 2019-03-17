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
        private static int BinaryMatch(byte[] input, byte[] pattern)
        {
            int sLen = input.Length - pattern.Length + 1;
            for (int i = 0; i < sLen; ++i)
            {
                bool match = true;
                for (int j = 0; j < pattern.Length; ++j)
                {
                    if (input[i + j] != pattern[j])
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                {
                    return i;
                }
            }
            return -1;
        }
        public async Task<byte[]> DoProxyRequest(HttpRequest httpRequest)
        {
            try
            {
                string httpRequestString = httpRequest.HttpString;
                string hostString = httpRequest.GetHeader("Host");
                Uri baseUri = new Uri("http://" + hostString);
                TcpClient proxyTcpClient = new TcpClient(baseUri.Host, baseUri.Port);
                NetworkStream proxyStream = proxyTcpClient.GetStream();
                byte[] requestInBytes = Encoding.ASCII.GetBytes(httpRequestString);
                await proxyStream.WriteAsync(requestInBytes, 0, requestInBytes.Length);
                //byte[] buffer = new byte[bufferSize];
                MemoryStream memory = new MemoryStream();
                //NetworkStream tmpStream = proxyTcpClient.GetStream();
                //this line slows down the app
                await Task.Run( async() => await proxyStream.CopyToAsync(memory));
                //await proxyStream.CopyToAsync(memory);
                proxyTcpClient.Dispose();
                proxyStream.Dispose();
                return memory.ToArray();
            }
            catch(ArgumentException err)
            {
                throw new BadRequestException(err.Message);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task WriteMessageWithBufferAsync(NetworkStream destinationStream, byte[] messageBytes, int buffer, bool removeImg = false)
        {
            if (removeImg)
            {
                //BUG HERE
                messageBytes = await ReplaceImages(messageBytes);
            }
            int index = 0;
            while (index < messageBytes.Length)
            {
                int diff =  messageBytes.Length - index;
                if (diff < buffer)
                {
                    await destinationStream.WriteAsync(messageBytes, index, diff);
                }
                else
                {

                    await destinationStream.WriteAsync(messageBytes, index, buffer);
                }
                index += buffer;
            }
        }
        private static async Task<byte[]> ReplaceImages(byte[] message)
        {
            MemoryStream memory = new MemoryStream(message);
            memory.Position = 0;
            if (message.Length == 0) throw new BadRequestException("Could not determine the stream");
            var index = BinaryMatch(message, Encoding.ASCII.GetBytes("\r\n\r\n")) + 4;
            var headers = Encoding.ASCII.GetString(message, 0, index);
            memory.Position = index;
            if (headers.Contains("Content-Type: image"))
            {
                //use memory to read the body. replace the image if settings say so
                byte[] placeholderBytes = File.ReadAllBytes(@"Assets\Placeholder.png");
                await memory.WriteAsync(placeholderBytes, 0, placeholderBytes.Length);
            }
            return memory.ToArray();
        }

        public async Task<byte[]> GetBytesFromReading(int bufferSize, NetworkStream stream)
        {
            byte[] buffer = new byte[bufferSize];
            //use memory stream to save all bytes
            MemoryStream memory = new MemoryStream();
            do
            {
                int readBytes = await stream.ReadAsync(buffer, 0, buffer.Length);
                await memory.WriteAsync(buffer, 0, readBytes);
            } while (stream.DataAvailable);
            return memory.ToArray();
        }
    }
}
