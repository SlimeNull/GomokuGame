namespace GomokuGame
{
    static class SimpleLog
    {
        public static void Log(string message)
        {
            Console.WriteLine(message);
        }

        public static void Log(string message, params object?[] args)
        {
            Console.WriteLine(message, args);
        }
    }
}