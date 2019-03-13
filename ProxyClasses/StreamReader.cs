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
        public async Task<string> HttpRequestAsync(string httpRequest, NetworkStream clientStream)
        {
            string result = string.Empty;
            string hostString = getBetween(httpRequest, "http://", "/");
            Uri baseUri = new Uri("http://" + hostString);
            TcpClient tcp = new TcpClient(baseUri.Host, baseUri.Port);
            using (var stream = tcp.GetStream())
            {
                tcp.SendTimeout = 500;
                tcp.ReceiveTimeout = 1000;
                // Send request headers
                var builder = new StringBuilder();
                builder.AppendLine(httpRequest);
                builder.AppendLine();
                var header = Encoding.ASCII.GetBytes(builder.ToString());
                await stream.WriteAsync(header, 0, header.Length);

                // receive data
                using (var memory = new MemoryStream())
                {
                    await stream.CopyToAsync(memory);
                    memory.Position = 0;
                    var data = memory.ToArray();

                    var index = BinaryMatch(data, Encoding.ASCII.GetBytes("\r\n\r\n")) + 4;
                    var headers = Encoding.ASCII.GetString(data, 0, index);
                    memory.Position = index;

                    if (headers.IndexOf("Content-Encoding: gzip") > 0)
                    {
                        using (GZipStream decompressionStream = new GZipStream(memory, CompressionMode.Decompress))
                        using (var decompressedMemory = new MemoryStream())
                        {
                            decompressionStream.CopyTo(decompressedMemory);
                            decompressedMemory.Position = 0;
                            result = Encoding.UTF8.GetString(decompressedMemory.ToArray());
                        }
                    }
                    else
                    {
                        await clientStream.WriteAsync(data, index, data.Length - index);
                        result = headers + Encoding.UTF8.GetString(data, index, data.Length - index);
                        //result = Encoding.GetEncoding("gbk").GetString(data, index, data.Length - index);
                    }
                }

                //Debug.WriteLine(result);
                return result;
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
    }
}
