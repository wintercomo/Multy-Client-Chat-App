using System;
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
        public async Task<byte[]> DoProxyRequest(HttpRequest httpRequest,NetworkStream clientStream, Int32 bufferSize = 1024)
        {
            string httpRequestString = httpRequest.HttpString;
            string hostString = httpRequest.GetHeader("Host");
            Uri baseUri = new Uri("http://" + hostString);
            TcpClient tcp = new TcpClient(baseUri.Host, baseUri.Port);
            var stream = tcp.GetStream();
            var header = Encoding.ASCII.GetBytes(httpRequestString);
            await stream.WriteAsync(header, 0, header.Length);
            byte[] buffer = new byte[bufferSize];
            MemoryStream memory = new MemoryStream();
            NetworkStream tmpStream = tcp.GetStream();
            await CheckResponseType(memory, tmpStream);
            await WriteByReadingStreamAsync(clientStream, stream, buffer, memory);
            //await stream.CopyToAsync(memory);
            stream.Dispose();
            tcp.Dispose();
            return memory.ToArray();
        }

        private static async Task WriteByReadingStreamAsync(NetworkStream clientStream, NetworkStream stream, byte[] buffer, MemoryStream memory)
        {
            do
            {
                int readBytes = await stream.ReadAsync(buffer, 0, buffer.Length);
                await clientStream.WriteAsync(buffer, 0, readBytes);
                await memory.WriteAsync(buffer, 0, readBytes);
            } while (stream.DataAvailable);
        }
        public async Task WriteMessageWithBufferAsync(NetworkStream clientStream, byte[] badRequestResponse)
        {
            int index = 0;
            int buffer = 13;
            while (index < badRequestResponse.Length)
            {
                int diff =  badRequestResponse.Length - index;
                if (diff < buffer)
                {
                    await clientStream.WriteAsync(badRequestResponse, index, diff);
                    string knownResponse = Encoding.ASCII.GetString(badRequestResponse, index, diff);
                    Console.WriteLine("SNDING: " + knownResponse);
                }
                else
                {
                    await clientStream.WriteAsync(badRequestResponse, index, buffer);
                    string knownResponse = Encoding.ASCII.GetString(badRequestResponse, index, buffer);
                    Console.WriteLine("SNDING: " + knownResponse);
                }
                index += buffer;
            }
        }
        private static async Task CheckResponseType(MemoryStream memory, NetworkStream tmpStream)
        {
            await tmpStream.CopyToAsync(memory);
            memory.Position = 0;
            var data = memory.ToArray();
            var index = BinaryMatch(data, Encoding.ASCII.GetBytes("\r\n\r\n")) + 4;
            var headers = Encoding.ASCII.GetString(data, 0, index);
            memory.Position = index;
            bool contains = headers.Contains("Content-Type: image");
            if (headers.IndexOf("Content-Type: image/png") > 0)
            {
                //use memory to read the body.
                Console.WriteLine("Image wanted");
            }
        }

        public async Task<byte[]> GetBytesFromReading(int bufferSize, NetworkStream stream)
        {
            byte[] buffer = new byte[bufferSize];
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
