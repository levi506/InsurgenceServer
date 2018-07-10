using System;

namespace InsurgenceServerCore.Logger
{
    public static class ErrorLog
    {

        public static void Initialize()
        {

        }
        public static DateTime LastError;
        public static void Log(object e)
        {
            Console.Error.WriteLineAsync(e.ToString());
        }
    }
}
