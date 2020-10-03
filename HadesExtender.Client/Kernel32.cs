﻿using System;
using System.Runtime.InteropServices;

namespace HadesExtender
{
    static class Kernel32
    {
        [DllImport("kernel32", SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        public static T GetProcAddress<T>(IntPtr hModule, string procedureName)
            where T : Delegate
        {
            return Marshal.GetDelegateForFunctionPointer<T>(GetProcAddress(hModule, procedureName));
        }

        [DllImport("kernel32", SetLastError = true)]
        public static extern IntPtr LoadLibrary(string fileName);
    }
}
