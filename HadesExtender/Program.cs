﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.Remoting;
using EasyHook;

namespace HadesExtender
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
                args = new[] { @"C:\Program Files (x86)\Steam\steamapps\common\Hades\x64\Hades.exe" };

            RemoteHooking.CreateAndInject(args[0],
                "",
                InProcessCreationFlags: 0,
                InjectionOptions.DoNotRequireStrongName,
                typeof(EntryPoint).Assembly.Location,
                typeof(EntryPoint).Assembly.Location,
                out int processID);

            Process.GetProcessById(processID).WaitForExit();
        }
    }
}
