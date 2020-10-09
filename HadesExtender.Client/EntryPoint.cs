using System;
using System.Threading;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using EasyHook;
using System.Runtime.InteropServices;

namespace HadesExtender
{
    public class EntryPoint : IEntryPoint
    {
        static IpcInterface server;

        static ProcessModule module;

        static DiaSymbolResolver resolver;

        static event Action OnEngineInitialized;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate IntPtr LoadLibraryADelegate(string fileName);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        delegate void ScreenManagerUpdateDelegate(IntPtr screenManager, float delta);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void InitLuaDelegate();

        ScriptManager scriptManager;

        [SuppressMessage("Style", "IDE0060", Justification = "Required by EasyHook")]
        public EntryPoint(RemoteHooking.IContext context, string channelName)
        {
            server = RemoteHooking.IpcConnectClient<IpcInterface>(channelName);
            Console.SetIn(server.In);
            Console.SetOut(new TextWriterWrapper(server.Out));
            Console.SetError(new TextWriterWrapper(server.Error));

            try
            {
                foreach (ProcessModule module in Process.GetCurrentProcess().Modules)
                {
                    Console.WriteLine($"Module: {module.ModuleName}");
                }
                Console.WriteLine();

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                throw;
            }
        }

        [SuppressMessage("Style", "IDE0060", Justification = "Required by EasyHook")]
        public void Run(RemoteHooking.IContext context, string channelName)
        {
            try
            {
                var kernel = GetKernelModule().BaseAddress;
                var loadLibraryAFunc = Kernel32.GetProcAddress(kernel, "LoadLibraryA");
                var hook = LocalHook.Create(loadLibraryAFunc, new LoadLibraryADelegate(LoadLibraryHook), null);
                hook.ThreadACL.SetExclusiveACL(Array.Empty<int>());

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
            using (HookRuntimeInfo.Handle)
            {
                var bypass = HookRuntimeInfo.Handle.HookBypassAddress;
                var method = Marshal.GetDelegateForFunctionPointer<LoadLibraryADelegate>(bypass);
                result = method.Invoke(filePath);
            }

            try
            {
                Console.WriteLine($"LoadLibraryHook called: {filePath}");
                module = GetEngineModule();
                resolver = new DiaSymbolResolver(module);

                {
                    var address = resolver.Resolve("?InitLua@ScriptManager@sgg@@SAXXZ");
                    var hook = LocalHook.Create(address, new InitLuaDelegate(InitLua), null);
                    hook.ThreadACL.SetExclusiveACL(Array.Empty<int>());
                    Console.WriteLine($"Hooked InitLua");
                }
                {
                    var address = resolver.Resolve("?Update@ScreenManager@sgg@@QEAAXM@Z");
                    var hook = LocalHook.Create(address, new ScreenManagerUpdateDelegate(ScreenManagerUpdate), null);
                    hook.ThreadACL.SetExclusiveACL(Array.Empty<int>());
                    Console.WriteLine($"Hooked ScreenManager");
                }

                scriptManager = new ScriptManager(resolver);
            } catch(Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
            }
            return result;
        }

        private void InitLua()
        {
            var bypass = HookRuntimeInfo.Handle.HookBypassAddress;
            var method = Marshal.GetDelegateForFunctionPointer<InitLuaDelegate>(bypass);
            method.Invoke();

            try
            {
                Console.WriteLine($"InitLua called");
                scriptManager.Init();
            } catch(Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
            }
        }

        private void ScreenManagerUpdate(IntPtr screenManager, float delta)
        {

            var bypass = HookRuntimeInfo.Handle.HookBypassAddress;
            var method = Marshal.GetDelegateForFunctionPointer<ScreenManagerUpdateDelegate>(bypass);
            method.Invoke(screenManager, delta);

            //Console.WriteLine($"ScreenManagerUpdate: {delta}");
        }
    }
}
