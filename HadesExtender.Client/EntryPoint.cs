using System;
using System.Threading;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using EasyHook;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Generic;

namespace HadesExtender
{
    public class EntryPoint : IEntryPoint
    {
        static ProcessModule module;

        static DiaSymbolResolver resolver;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate IntPtr LoadLibraryADelegate(string fileName);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        delegate void ScreenManagerUpdateDelegate(IntPtr screenManager, float delta);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void InitLuaDelegate();

        ScriptManager scriptManager;

        static void AttachToParentConsole()
        {
            Kernel32.FreeConsole();
            Kernel32.AttachConsole(Kernel32.ATTACH_PARENT_PROCESS);
            Console.SetIn(new StreamReader(Console.OpenStandardInput()));
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
            Console.SetError(new StreamWriter(Console.OpenStandardError()) { AutoFlush = true });
        }

        [SuppressMessage("Style", "IDE0060", Justification = "Required by EasyHook")]
        public EntryPoint(RemoteHooking.IContext context)
        {
            AttachToParentConsole();
        }
        
        public bool Is64Bit => IntPtr.Size == 8;

        [SuppressMessage("Style", "IDE0060", Justification = "Required by EasyHook")]
        public void Run(RemoteHooking.IContext context)
        {
            try
            {
                var kernel = GetKernelModule().BaseAddress;
                var loadLibraryAFunc = Kernel32.GetProcAddress(kernel, "LoadLibraryA");
                var hook = LocalHook.Create(loadLibraryAFunc, new LoadLibraryADelegate(LoadLibraryHook), null);
                hook.ThreadACL.SetExclusiveACL(Array.Empty<int>());
                Hooks.Add("LoadLibraryA", hook);

                if (Is64Bit)
                {
                    Kernel32.LoadLibrary("EngineWin64s.dll");
                } else
                {
                    Kernel32.LoadLibrary("EngineWin32s.dll");
                }

                foreach (ProcessModule module in Process.GetCurrentProcess().Modules)
                {
                    Console.WriteLine($"Module: {module.ModuleName}");
                }
                Console.WriteLine();

                module = GetEngineModule();
                resolver = new DiaSymbolResolver(module);
                PdbSymbolImporter.ImportSymbols(resolver);
                LuaHelper.InitHelper(resolver);

                Hook<InitLuaDelegate>("?InitLua@ScriptManager@sgg@@SAXXZ", InitLua);
                Hook<ScreenManagerUpdateDelegate>("?Update@ScreenManager@sgg@@QEAAXM@Z", ScreenManagerUpdate);

                scriptManager = new ScriptManager(resolver);
                Console.WriteLine($"Created ScriptManager");
                RemoteHooking.WakeUpProcess();

                while (true)
                {
                    var code = Console.ReadLine();
                    Console.WriteLine("> {0}", code);
                    scriptManager.Eval(code);
                    Thread.Sleep(500);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                throw;
            }
        }
        //Add hooks to dictionary to prevent them being GC'd
        Dictionary<string, LocalHook> Hooks = new Dictionary<string, LocalHook>();
        void Hook<T>(string name, T callback) where T : Delegate
        {
            var address = resolver.Resolve(name);
            var hook = LocalHook.Create(address, callback, null);
            hook.ThreadACL.SetExclusiveACL(Array.Empty<int>());
            Hooks.Add(name, hook);
            Console.WriteLine($"Hooked {name}");
        }

        ProcessModule GetEngineModule()
        {
            foreach (ProcessModule module in Process.GetCurrentProcess().Modules)
            {
                if (module.ModuleName.StartsWith("EngineWin"))
                    return module;
            }

            throw new MissingModuleException("EngineWin");
        }

        ProcessModule GetKernelModule()
        {
            foreach (ProcessModule module in Process.GetCurrentProcess().Modules)
            {
                if (module.ModuleName.ToLower() == "kernel32.dll")
                    return module;
            }

            throw new MissingModuleException("kernel32");
        }

        private IntPtr LoadLibraryHook(string filePath)
        {
            IntPtr result;

            var bypass = HookRuntimeInfo.Handle.HookBypassAddress;
            var method = Marshal.GetDelegateForFunctionPointer<LoadLibraryADelegate>(bypass);
            result = method.Invoke(filePath);

            Console.WriteLine($"LoadLibraryHook called: {filePath}");

            return result;
        }

        private void InitLua()
        {
            try
            {
                Console.WriteLine($"InitLua Start");

                var bypass = HookRuntimeInfo.Handle.HookBypassAddress;
                var method = Marshal.GetDelegateForFunctionPointer<InitLuaDelegate>(bypass);
                method.Invoke();

                Console.WriteLine($"InitLua End");
                scriptManager.Init();
                LuaHelper.OpenLibraries(scriptManager.State);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
            }
        }

        int ScreenManagerUpdateCount = 0;
        private void ScreenManagerUpdate(IntPtr screenManager, float delta)
        {

            var bypass = HookRuntimeInfo.Handle.HookBypassAddress;
            var method = Marshal.GetDelegateForFunctionPointer<ScreenManagerUpdateDelegate>(bypass);
            method.Invoke(screenManager, delta);
            if (ScreenManagerUpdateCount < 10)
            {
                Console.WriteLine($"ScreenManagerUpdate: {delta}");
                ScreenManagerUpdateCount++;
            }
        }
    }
}
