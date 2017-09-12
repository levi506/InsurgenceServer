using System;
using System.IO;

namespace InsurgenceServer.Logger
{
    public static class ErrorLog
    {

        public static void Initialize()
        {
            
        }
        public static DateTime LastError;
        public static void Log(object e)
        {
            Console.WriteLine(e.ToString());
        }
    }
}
