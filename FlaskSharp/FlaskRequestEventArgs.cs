using System;

namespace FlaskSharp
{
    public class FlaskRequestEventArgs : EventArgs
    {
        public FlaskRequestEventArgs(FlaskConnection connection, HttpRequest request)
        {
            Connection = connection;
            Request = request;
        }

        public FlaskConnection Connection { get; }
        public HttpRequest Request { get; }
    }
}