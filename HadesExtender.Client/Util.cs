using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
    }
}
