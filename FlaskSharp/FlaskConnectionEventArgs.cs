using System;
using System.Net;

namespace FlaskSharp
{
    public class FlaskConnectionEventArgs : EventArgs
    {
        public FlaskConnectionEventArgs(FlaskConnection connection)
        {
            Connection = connection;
        }

        public FlaskConnection Connection { get; }
    }
}