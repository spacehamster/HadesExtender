using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace HadesExtender
{
    public class Util
    {
        public bool Is64Bit => IntPtr.Size == 8;

        public static ProcessModule GetModule(string name)
        {
            foreach (ProcessModule module in Process.GetCurrentProcess().Modules)
            {
                if (module.ModuleName.ToLower() == name.ToLower())
                    return module;
            }

            throw new MissingModuleException(name);
        }

        public static void LoadExtenderLibrary(string name)
        {
            var assemblyPath = Assembly.GetExecutingAssembly().Location;
            var assemblyDir = Path.GetDirectoryName(assemblyPath);
            Kernel32.LoadLibrary(Path.Combine(assemblyDir, name));
        }

        public static string ExtenderDirectory => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    }
}
