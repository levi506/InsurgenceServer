using System;
using System.Buffers;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InsurgenceServerCore.ClientHandler
{
    public class ListenPipe
    {
        private readonly Socket _socket;
        private readonly Pipe _pipe;
        private Client _client;

        public delegate Task OnCompleteMessageDelegate(string s);
        public event OnCompleteMessageDelegate OnCompleteMessage;

        public ListenPipe(Client client, Socket socket, Pipe pipe)
        {
            _socket = socket;
            _pipe = pipe;
            _client = client;
        }

        public async Task Run()
        {
#pragma warning disable 4014
            PipeListener(_socket, _pipe.Writer);
            PipeHandler(_pipe.Reader);
#pragma warning restore 4014
        }

        private async Task PipeListener(Socket socket, PipeWriter writer)
        {
            const int bufferSize = 512;
            while (_client.Connected)
            {
                var memory = writer.GetMemory(bufferSize);
                try
                {
                    if (!socket.Connected)
                    {
                        await _client.Disconnect();
                        return;
                    }

                    int bytesRead;
                    try
                    {
                        bytesRead = await socket.ReceiveAsync(memory, SocketFlags.None);
                    }
                    catch (SocketException)
                    {
                        await _client.Disconnect();
                        break;
                    }
                    if (bytesRead == 0)
                    {
                        Console.WriteLine("Breaking");
                        break;
                    }

                    writer.Advance(bytesRead);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    //
                }

                await writer.FlushAsync();
            }
        }

        private static string GetAsciiString(ReadOnlySequence<byte> buffer)
        {
            if (buffer.IsSingleSegment)
            {
                return Encoding.ASCII.GetString(buffer.First.Span);
            }

            return string.Create((int)buffer.Length, buffer, (span, sequence) =>
            {
                foreach (var segment in sequence)
                {
                    Encoding.ASCII.GetChars(segment.Span, span);

                    span = span.Slice(segment.Length);
                }
            });
        }

        private StringBuilder _message = new StringBuilder();

        // ReSharper disable once FunctionRecursiveOnAllPaths
        private async Task PipeHandler(PipeReader reader)
        {
            try
            {
                var               result = await reader.ReadAsync();
                var               buffer = result.Buffer;
                SequencePosition? position;
                do
                {
                    position = buffer.PositionOf((byte) '>');
                    if (position.HasValue)
                    {
                        var pos   = position.Value.GetInteger();
                        var bytes = buffer.Slice(0, position.Value);
                        _message.Append(GetAsciiString(bytes));
                        OnCompleteMessage?.Invoke(_message.ToString());
                        buffer   = buffer.Slice(buffer.GetPosition(1, position.Value));
                        _message = new StringBuilder();
                    }
                    else
                    {
                        _message.Append(GetAsciiString(buffer).Replace("\n", ""));
                    }
                } while (position.HasValue);

                reader.AdvanceTo(buffer.Start, buffer.End);
                await PipeHandler(reader);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await PipeHandler(reader);
            }
        }
    }
}