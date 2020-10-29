using System.Diagnostics;
using EasyHook;

namespace HadesExtender
{
    /*
     * DebugBootstrapper will look for an attached debugger and if found, detach it and 
     * tell the client to launch it's own debugger. It is a seperate program from the normal launcher because 
     * it requires a dependency on envdte
     */
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
                args = new[] { @"C:\Program Files (x86)\Steam\steamapps\common\Hades\x64\Hades.exe" };
            string clientArgs = "";
            if (Debugger.IsAttached)
            {
                DebugHelper.DetachDebugger();
                clientArgs += "--launch-debugger";
            }            
            RemoteHooking.CreateAndInject(args[0],
                clientArgs,
                InProcessCreationFlags: 0,
                InjectionOptions.DoNotRequireStrongName,
                typeof(EntryPoint).Assembly.Location,
                typeof(EntryPoint).Assembly.Location,
                out int processID);

            Process.GetProcessById(processID).WaitForExit();
        }
    }
}
