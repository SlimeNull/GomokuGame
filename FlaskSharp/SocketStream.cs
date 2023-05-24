using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FlaskSharp
{
    internal class TcpSocketStream : Stream
    {
        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => true;

        public override long Length => throw new NotSupportedException();

        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

        public Socket Socket { get; }

        private TcpSocketStream(Socket socket)
        {
            if (socket.SocketType != SocketType.Stream)
                throw new ArgumentException("Must be Stream socket", nameof(socket));

            Socket = socket;
        }

        public override void Flush() { }
        public override int Read(byte[] buffer, int offset, int count)
        {
            return Socket.Receive(buffer, offset, count, SocketFlags.None);
        }
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count)
        {
            Socket.Send(buffer, offset, count, SocketFlags.None);
        }

        public static TcpSocketStream Of(Socket socket)
        {
            return new TcpSocketStream(socket);
        }
    }
}
