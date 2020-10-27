using Reloaded.Hooks;
using Reloaded.Hooks.Definitions.Enums;
using Reloaded.Hooks.Tools;
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

        //Reloaded.Hooks 2.4.0 underestimates the size of AsmHooks and can sometimes throw exceptions while trying to hook
        //Remove this once the bug is fixed in Reloaded.Hooks
        public static AsmHook HookSafe(string[] asmCode, long functionAddress, AsmHookBehaviour behaviour = AsmHookBehaviour.ExecuteFirst)
        {
            var asmBytes = Utilities.Assembler.Assemble(asmCode);
            var _is64Bit = IntPtr.Size == 8;
            int maxJmpSize = 7; // Maximum size of jmp opcode.
            var hookLength = Utilities.GetHookLength((IntPtr)functionAddress, maxJmpSize, _is64Bit);
            hookLength += _is64Bit ? 24 : 12; //AsmHook under estimates the size of hooks by 3 pointers
            return new AsmHook(asmBytes, functionAddress, behaviour, hookLength);
        }

        public static string ExtenderDirectory => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    }
}
