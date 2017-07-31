using System;

namespace InsurgenceServer.Logger
{
    public static class Logger
    {
        public static void Log(string message)
        {
            Console.WriteLine($"{DateTime.UtcNow:O} - {message}");
        }
    }
}