using System;
using System.Runtime.InteropServices;

namespace HadesExtender
{
    class SEHExceptionHandler
    {
        const uint EXCEPTION_CONTINUE_EXECUTION = uint.MaxValue;
        const uint EXCEPTION_CONTINUE_SEARCH = 0;
        [DllImport("kernel32", SetLastError = true)]
        static extern IntPtr AddVectoredExceptionHandler(uint first, HandleExceptionDelegate handler);

        private static HandleExceptionDelegate callback;
        delegate uint HandleExceptionDelegate(IntPtr exceptionInfo);

        public static void Register()
        {
            callback = new HandleExceptionDelegate(HandleException);
            AddVectoredExceptionHandler(0, callback);
        }
        static bool HandlingException;
        static uint HandleException(IntPtr exceptionInfo)
        {
            if (HandlingException) return EXCEPTION_CONTINUE_SEARCH;
            HandlingException = true;
            Console.WriteLine("Segfault occured");
            if (ScriptManager.LuaValid)
            {
                var stack = ScriptManager.StackTrace();
                Console.WriteLine(stack);
            }
            HandlingException = false;
            return EXCEPTION_CONTINUE_SEARCH;
        }
    }
}
