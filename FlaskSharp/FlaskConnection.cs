using System.Net;

namespace FlaskSharp
{
    public class FlaskConnection
    {
        public FlaskConnection(EndPoint? localEndPoint, EndPoint? remoteEndPoint)
        {
            LocalEndPoint = localEndPoint;
            RemoteEndPoint = remoteEndPoint;
        }

        public EndPoint? LocalEndPoint { get; }
        public EndPoint? RemoteEndPoint { get; }
    }
}