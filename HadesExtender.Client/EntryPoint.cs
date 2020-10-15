﻿using System;
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
        static IpcInterface server;

        static ProcessModule module;

        static DiaSymbolResolver resolver;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate IntPtr LoadLibraryADelegate(string fileName);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate IntPtr StartAppDelegate(uint param);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate IntPtr InitWindowDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void AppMainDelegate(int argc, IntPtr argv, IntPtr app);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        delegate void ScreenManagerUpdateDelegate(IntPtr screenManager, float delta);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void InitLuaDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void ScriptManagerInitDelegate();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        delegate void ResetAppDelegate(IntPtr app, byte param1, IntPtr param2);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void CursorManagerInitializeDelegate(IntPtr platformCusorFactory);

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
        public EntryPoint(RemoteHooking.IContext context, string channelName)
        {
            server = RemoteHooking.IpcConnectClient<IpcInterface>(channelName);
            AttachToParentConsole();
        }
        [SuppressMessage("Style", "IDE0060", Justification = "Required by EasyHook")]
        public void Run(RemoteHooking.IContext context, string channelName)
        {
            try
            {
                foreach (ProcessModule module in Process.GetCurrentProcess().Modules)
                {
                    Console.WriteLine($"Module: {module.ModuleName}");
                }
                Console.WriteLine();

                var kernel = GetKernelModule().BaseAddress;
                var loadLibraryAFunc = Kernel32.GetProcAddress(kernel, "LoadLibraryA");
                var hook = LocalHook.Create(loadLibraryAFunc, new LoadLibraryADelegate(LoadLibraryHook), null);
                hook.ThreadACL.SetExclusiveACL(Array.Empty<int>());
                Hooks.Add("LoadLibraryA", hook);

                Kernel32.LoadLibrary("EngineWin64s.dll");
                module = GetEngineModule();
                resolver = new DiaSymbolResolver(module);
                PdbSymbolImporter.ImportSymbols(resolver);

                Hook<InitLuaDelegate>("?InitLua@ScriptManager@sgg@@SAXXZ", InitLua);
                Hook<ScriptManagerInitDelegate>("?Init@ScriptManager@sgg@@SAXXZ", ScriptManagerInit);
                Hook<ScreenManagerUpdateDelegate>("?Update@ScreenManager@sgg@@QEAAXM@Z", ScreenManagerUpdate);
                Hook<CursorManagerInitializeDelegate>("?Initialize@CursorManager@sgg@@SAXPEAVPlatformCursorFactory@2@@Z", CursorManagerInitialize);
                Hook<ResetAppDelegate>("?Reset@App@sgg@@QEAAX_NAEBV?$basic_string@DVallocator_forge@eastl@@@eastl@@@Z", ResetApp);
                Hook<StartAppDelegate>("StartApp", StartApp);
                Hook<InitWindowDelegate>("InitWindow", InitWindow);
                Hook<AppMainDelegate>("AppMain", AppMain);

                scriptManager = new ScriptManager(resolver);
                Console.WriteLine($"Created ScriptManager");
                RemoteHooking.WakeUpProcess();

                while (true)
                {
                    server.Ping();
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
                if (module.ModuleName.StartsWith("EngineWin64"))
                    return module;
            }

            throw new MissingModuleException("EngineWin64");
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

        private void ScriptManagerInit()
        {
            try
            {
                Console.WriteLine($"ScriptManagerInit Start");

                var bypass = HookRuntimeInfo.Handle.HookBypassAddress;
                var method = Marshal.GetDelegateForFunctionPointer < ScriptManagerInitDelegate>(bypass);
                method.Invoke();

                Console.WriteLine($"ScriptManagerInit End");
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


        private void ResetApp(IntPtr app, byte param1, IntPtr param2)
        {
            Console.WriteLine($"ResetApp Start");
            var bypass = HookRuntimeInfo.Handle.HookBypassAddress;
            var method = Marshal.GetDelegateForFunctionPointer<ResetAppDelegate>(bypass);
            method.Invoke(app, param1, param2);

            Console.WriteLine($"ResetApp End");
        }

        private void CursorManagerInitialize(IntPtr platformCursorManager)
        {
            Console.WriteLine($"CursorManagerInitialize Start");
            var bypass = HookRuntimeInfo.Handle.HookBypassAddress;
            var method = Marshal.GetDelegateForFunctionPointer<CursorManagerInitializeDelegate>(bypass);
            method.Invoke(platformCursorManager);

            Console.WriteLine($"CursorManagerInitialize End");
        }
        private IntPtr StartApp(uint param)
        {
            Console.WriteLine($"StartApp Start {param}");

            var bypass = HookRuntimeInfo.Handle.HookBypassAddress;
            var method = Marshal.GetDelegateForFunctionPointer<StartAppDelegate>(bypass);
            var result = method.Invoke(param);

            Console.WriteLine($"StartApp End");

            return result;
        }

        private IntPtr InitWindow()
        {
            Console.WriteLine($"InitWindow Start");
            var bypass = HookRuntimeInfo.Handle.HookBypassAddress;
            var method = Marshal.GetDelegateForFunctionPointer<InitWindowDelegate>(bypass);
            var result = method.Invoke();

            Console.WriteLine($"InitWindow End");

            return result;
        }

        private void AppMain(int argc, IntPtr argv, IntPtr app)
        {
            Console.WriteLine($"AppMain Start");

            var bypass = HookRuntimeInfo.Handle.HookBypassAddress;
            var method = Marshal.GetDelegateForFunctionPointer<AppMainDelegate>(bypass);
            method.Invoke(argc, argv, app);

            Console.WriteLine($"AppMain End");
        }


    }
}
