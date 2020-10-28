using EasyHook;
using Reloaded.Hooks.Definitions.Enums;
using Reloaded.Hooks.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace HadesExtender
{
    public class CustomLuaRuntimeManager
    {
        List<string> unresolvedSymbols = new List<string>();
        Dictionary<string, object> luahooks = new Dictionary<string, object>();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate LuaState LuaLNewStateDelegate();
        public static LuaLNewStateDelegate luaL_newstate = null;


        public unsafe void Init(SymbolResolver resolver, LuaInterface* luaInterface, IntPtr l_msghandler, IntPtr l_panic, bool* enableMessageHook)
        {
            CloseLua(luaInterface);
            HookLuaApi(resolver);
            OpenLua(luaInterface, l_msghandler, l_panic, enableMessageHook);
        }

        private void HookLuaApi(SymbolResolver resolver)
        {
            Kernel32.LoadLibrary(Path.Combine(Util.ExtenderDirectory, "Lua.dll"));
            var luaModule = Util.GetModule("Lua.dll");
            var luaResolver = new DiaSymbolResolver(luaModule);
            luaL_newstate = luaResolver.ResolveFunction<LuaLNewStateDelegate>("luaL_newstate");
            using var sw = new StreamWriter("PatchLog.txt");
            var useEasyhook = false;
            var ignoreSymbols = new string[] {
                "luaopen_debug"
            };
            var symbols = resolver.FindSymbolsMatching(new Regex("lua*"))
                .Concat(resolver.FindSymbolsMatching(new Regex(@"\?lua*")))
                .Where(symbol => !ignoreSymbols.Contains(symbol));
            foreach (var symbol in symbols)
            {
                var source = resolver.Resolve(symbol);
                if (!luaResolver.TryResolve(symbol, out var target))
                {
                    RegisterErrorHook(symbol, source);
                    sw.WriteLine($"Could not find symbol {symbol} in lua.dll");
                    continue;
                }
                if (useEasyhook)
                {
                    try
                    {
                        var hook = LocalHook.CreateUnmanaged(source, target, IntPtr.Zero);
                        hook.ThreadACL.SetExclusiveACL(Array.Empty<int>());
                        luahooks[symbol] = hook;
                        sw.WriteLine($"hooked lua function {symbol}");
                    }
                    catch (NotSupportedException ex)
                    {
                        sw.WriteLine($"Could not hook {symbol} - {ex.Message}");
                    }
                }
                else
                {
                    var asm = new string[] {
                        $"use64",
                        Utilities.GetAbsoluteJumpMnemonics(target, is64bit:true)
                    };
                    var hook = Util.HookSafe(asm, source.ToInt64(), AsmHookBehaviour.DoNotExecuteOriginal).Activate();
                    luahooks[symbol] = hook;
                    sw.WriteLine($"hooked lua function {symbol}. 0x{source.ToInt64():X8} -> 0x{target.ToInt64():X8}");
                }
            }
        }

        private unsafe void CloseLua(LuaInterface* luaInterface)
        {
            Console.WriteLine($"Closing lua.");
            if (luaInterface->destroyed == false)
            {
                LuaBindings.lua_close(luaInterface->state);
                luaInterface->state = IntPtr.Zero;
                luaInterface->destroyed = true;
                luaInterface->msghander = 0;
            }
        }

        private unsafe void OpenLua(LuaInterface* luaInterface, IntPtr l_msghandler, IntPtr l_panic, bool* enableMessageHook)
        {
            Console.WriteLine($"Opening lua. Message hooks enabled={*enableMessageHook}");
            luaInterface->state = CustomLuaRuntimeManager.luaL_newstate();
            luaInterface->destroyed = false;
            if (!*enableMessageHook)
            {
                LuaBindings.luaL_openlibs(luaInterface->state);
            }
            else
            {
                LuaBindings.lua_pushcclosure(luaInterface->state, l_msghandler, 0);
                var top = LuaBindings.lua_gettop(luaInterface->state);
                luaInterface->msghander = top;
                LuaBindings.lua_atpanic(luaInterface->state, l_panic);
                LuaBindings.luaL_openlibs(luaInterface->state);
                LuaBindings.luaopen_debug(luaInterface->state);
            }
        }

        [Reloaded.Hooks.Definitions.X64.Function(Reloaded.Hooks.Definitions.X64.CallingConventions.Microsoft)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void OnErrorDelegate(int index);

        public void OnError(int index)
        {
            var name = unresolvedSymbols[index];
            Console.WriteLine($"Unresolved lua symbol called: {name}");
        }
        Reloaded.Hooks.X64.ReverseWrapper<OnErrorDelegate> OnErrorWrapper;

        public void RegisterErrorHook(string symbol, IntPtr source)
        {
            if(OnErrorWrapper == null)
            {
                OnErrorWrapper = new Reloaded.Hooks.X64.ReverseWrapper<OnErrorDelegate>(OnError);
            }

            var asm = new string[] {
                        $"use64",
                        $"mov ecx, {unresolvedSymbols.Count}",
                        Utilities.GetAbsoluteJumpMnemonics(OnErrorWrapper.WrapperPointer, is64bit:true)
                    };
            var hook = Util.HookSafe(asm, source.ToInt64(), AsmHookBehaviour.DoNotExecuteOriginal).Activate();
            luahooks[symbol] = hook;
            unresolvedSymbols.Add(symbol);
        }
    }
}
