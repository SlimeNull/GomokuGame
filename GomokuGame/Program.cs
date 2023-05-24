using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using FlaskSharp;

namespace GomokuGame
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            GomokuGameApp app = new GomokuGameApp();

            app.ListenAddress = IPAddress.Loopback;
            if (int.TryParse(Environment.GetEnvironmentVariable("PORT"), out int port))
                app.ListenPort = port;

            app.JsonSerializerOptions = new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            app.Connected += App_Connected;
            app.Responded += App_Responded;
            
            SimpleLog.Log("Listening at {0}:{1}", app.ListenAddress, app.ListenPort);

            app.Run();
        }

        private static void App_Connected(object? sender, FlaskConnectionEventArgs e)
        {
            SimpleLog.Log("Connection established with {0}", e.Connection.LocalEndPoint);
        }

        private static void App_Responded(object? sender, FlaskResponseEventArgs e)
        {
            SimpleLog.Log("Thread {0} sent response to {1} for {2}",
                Thread.CurrentThread.ManagedThreadId, e.Connection.LocalEndPoint, e.Request.Url.PathAndQuery);
        }
    }
}