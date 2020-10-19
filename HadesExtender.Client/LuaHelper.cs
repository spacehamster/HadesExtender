using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace HadesExtender
{
    /*
     * LuaHelper is a native proxy library that exports lua symbols and allows 
     * lua calls to LuaHelper to be redirected to the engine's lua runtime.
     * It must be initialized with a list of engine symbol names and addresses.
     */ 
    public static class LuaHelper
    {
        [DllImport("LuaHelper", CallingConvention=CallingConvention.Cdecl)]
        static extern void InitProxy(string[] names, IntPtr[] addresses, int count);
        static IntPtr luaopen_package;
        static IntPtr luaopen_io;
        static IntPtr luaopen_os;

        public static void InitHelper(SymbolResolver engineResolver)
        {
            Util.LoadExtenderLibrary("LuaHelper.dll");
            var helperModule = Util.GetModule("LuaHelper.dll");
            var helperResolver = new DbgHelpSymbolResolver(helperModule);
            var symbols = helperResolver.FindSymbolsMatching(new Regex("lua*")).ToArray();
            if (symbols.Length == 0)
            {
                throw new UnresolvedSymbolException("Could not find LuaHelper symbols");
            }
            var addresses = symbols
                .Select(name => engineResolver.Resolve(name))
                .ToArray();
            InitProxy(symbols, addresses, symbols.Length);
            luaopen_package = helperResolver.Resolve("luaopen_package");
            luaopen_io = helperResolver.Resolve("luaopen_io");
            luaopen_os = helperResolver.Resolve("luaopen_os");
        }
        public static void OpenLibraries(LuaState state)
        {
            if (state == IntPtr.Zero) throw new Exception("Lua state is null");
            LuaBindings.luaL_requiref(state, "package", luaopen_package, 1);
            LuaBindingMacros.lua_pop(state, 1);
            LuaBindings.luaL_requiref(state, "io", luaopen_io, 1);
            LuaBindingMacros.lua_pop(state, 1);
            LuaBindings.luaL_requiref(state, "os", luaopen_os, 1);
            LuaBindingMacros.lua_pop(state, 1);
        }
    }
}
