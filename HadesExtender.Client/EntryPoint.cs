using System;
using System.Threading;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using EasyHook;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace HadesExtender
{
    public class EntryPoint : IEntryPoint
    {
        static ProcessModule module;

        static DiaSymbolResolver resolver;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate IntPtr LoadLibraryADelegate(string fileName);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void ScriptManagerUpdateDelegate(float delta);

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
                var args = Environment.GetCommandLineArgs();
                bool enableDebug = false;
                foreach(var arg in args)
                {
                    if(arg == "--launch-debugger")
                    {
                        Debugger.Launch();
                    }
                    if (arg == "-d" || arg == "--enable-debug")
                    {
                        Debugger.Launch();
                    }
                }
                var kernel = GetKernelModule().BaseAddress;
                var loadLibraryAFunc = Kernel32.GetProcAddress(kernel, "LoadLibraryA");
                var hook = LocalHook.Create(loadLibraryAFunc, new LoadLibraryADelegate(LoadLibraryHook), null);
                hook.ThreadACL.SetExclusiveACL(Array.Empty<int>());
                Hooks.Add("LoadLibraryA", hook);

                if (Is64Bit)
                {
                    Kernel32.LoadLibrary("EngineWin64s.dll");
                }
                else
                {
                    Kernel32.LoadLibrary("EngineWin32s.dll");
                }
                foreach (ProcessModule module in Process.GetCurrentProcess().Modules)
                {
                    Console.WriteLine($"Module: {module.ModuleName}");
                }
                module = GetEngineModule();
                resolver = new DiaSymbolResolver(module);
#if DEBUG
                LogEngineSymbols(resolver);
#endif
                PdbSymbolImporter.ImportSymbols(resolver);
                LuaHelper.InitHelper(resolver);

                Hook<InitLuaDelegate>("?InitLua@ScriptManager@sgg@@SAXXZ", InitLua);
                Hook<ScriptManagerUpdateDelegate>("?Update@ScriptManager@sgg@@SAXAEBM@Z", ScriptManagerUpdate);

                scriptManager = new ScriptManager(resolver, enableDebug);
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
        void LogEngineSymbols(SymbolResolver engineResolver)
        {
            var engineSymbols = engineResolver.FindSymbolsMatching(new Regex("lua*"));
            File.WriteAllLines("engine_symbols.txt", engineSymbols);

            Util.LoadExtenderLibrary("LuaHelper.dll");
            var helperModule = Util.GetModule("LuaHelper.dll");
            var helperResolver = new ExportResolver(helperModule);

            var symbols = helperResolver.FindSymbolsMatching(new Regex("lua*"));
            var missingSymbols = symbols.
                Where(name => !engineResolver.TryResolve(name, out var _));

            File.WriteAllLines("missing_symbols.txt", missingSymbols);
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
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
            }
        }

        private void ScriptManagerUpdate(float delta)
        {
            var bypass = HookRuntimeInfo.Handle.HookBypassAddress;
            var method = Marshal.GetDelegateForFunctionPointer<ScriptManagerUpdateDelegate>(bypass);
            method.Invoke(delta);
            scriptManager.Update();
        }
    }
}
