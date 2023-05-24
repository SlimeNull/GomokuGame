using System;

namespace FlaskSharp
{
    public class FlaskResponseEventArgs : EventArgs
    {
        public FlaskResponseEventArgs(FlaskConnection connection, HttpRequest request, HttpResponse response)
        {
            Connection = connection;
            Request = request;
            Response = response;
        }

        public FlaskConnection Connection { get; }
        public HttpRequest Request { get; }
        public HttpResponse Response { get; }
    }
}