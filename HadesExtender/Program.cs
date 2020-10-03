using System;
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

            string channel = null;
            var server     = new IpcInterface(Console.In, Console.Out, Console.Error);

            RemoteHooking.IpcCreateServer(ref channel, WellKnownObjectMode.Singleton, server);
            RemoteHooking.CreateAndInject(args[0],
                "",
                InProcessCreationFlags: 0,
                InjectionOptions.DoNotRequireStrongName,
                typeof(EntryPoint).Assembly.Location,
                typeof(EntryPoint).Assembly.Location,
                out int processID,
                channel
            );

            Process.GetProcessById(processID).WaitForExit();
        }

        static ManagementBaseObject GetManagementObjectForProcess(Process process)
        {
            var query           = $"select * from Win32_Process where ProcessId = {process.Id}";
            using var searcher  = new ManagementObjectSearcher(query);
            using var processes = searcher.Get();
            return processes.OfType<ManagementBaseObject>().FirstOrDefault();
        }
    }
}
