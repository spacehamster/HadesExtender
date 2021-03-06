﻿using EasyHook;
using Reloaded.Hooks;
using Reloaded.Hooks.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace HadesExtender
{
    public unsafe class ScriptManager
    {
        //TODO: Make config
        const bool CustomLuaRuntime = true;
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate int DBSetHookDelegate(LuaState L);
        DBSetHookDelegate db_sethook;



        SymbolResolver Resolver;
        public Lua lua;
        delegate void LuaFunc(LuaState L);
        static LuaInterface* luaInterface;
        public static LuaState State => luaInterface->state;
        bool* enableMessageHook;
        IntPtr l_msghandler;
        IntPtr l_panic;
        CustomLuaRuntimeManager customRuntime;
        bool DebugEnabled;
        public ScriptManager(SymbolResolver resolver, bool enableDebug)
        {
            DebugEnabled = enableDebug;
            Console.WriteLine("Initializing ScriptManager");
            Resolver = resolver;
            luaInterface = resolver.Resolve<LuaInterface>("?LUA_INTERFACE@ScriptManager@sgg@@2ULua@@A");
            enableMessageHook = resolver.Resolve<bool>("?EnableLuaMessageHook@ConfigOptions@sgg@@2_NA");
            l_msghandler = resolver.Resolve("l_msghandler");
            l_panic = resolver.Resolve("l_panic");
            lua = new Lua(resolver, luaInterface);
            if (CustomLuaRuntime)
            {
                customRuntime = new CustomLuaRuntimeManager();
                customRuntime.Init(resolver, luaInterface, l_msghandler, l_panic, enableMessageHook);
            }
        }

        public void Init()
        {

            Console.WriteLine("Registered TestLog function");
            lua.RegisterFunction<LuaFunc>("TestLog", TestLog);
            Console.WriteLine("Registered TesValue global");
            lua.SetGlobal("TestValue", (double)5);
            if (!CustomLuaRuntime)
            {
#pragma warning disable CS0162 // Unreachable code detected
                LuaHelper.OpenLibraries(State);
#pragma warning restore CS0162 // Unreachable code detected
            } else
            {
                customRuntime.OpenLibraries(State);
                //Restore debug.sethook function, engine stubs it at runtime.
                var top = LuaBindings.lua_gettop(State);
                LuaBindings.lua_getglobal(State, "debug");
                LuaBindings.lua_pushcclosure(State, customRuntime.db_sethook, 0);
                LuaBindings.lua_setfield(State, -2, "sethook");
                LuaBindings.lua_settop(State, top);
            }
            if (DebugEnabled)
            {
                Console.WriteLine("Loading debug scripts");
                var luadir = Path.Combine(Util.ExtenderDirectory, "lua_modules");
                lua.Eval(string.Format(@"package.path = package.path .. "";{0}""",
                    $@"{Util.ExtenderDirectory}\?.lua".Replace(@"\", @"\\")));
                lua.Eval(string.Format(@"package.path = package.path .. "";{0}""",
                    $@"{luadir}\share\lua\5.2\?.lua".Replace(@"\", @"\\")));
                lua.Eval(string.Format(@"package.cpath = package.cpath .. "";{0}""",
                    $@"{luadir}\lib\lua\5.2\?.dll".Replace(@"\", @"\\")));
                if (File.Exists($"{Util.ExtenderDirectory}/InitDebugging.lua"))
                {
                    lua.LoadFile($"{Util.ExtenderDirectory}/InitDebugging.lua");
                }
            }

            RegisterLoadBanks();
        }
        void RegisterLoadBanks()
        {
            lua.RegisterFunction<LuaFunc>("LoadBank", LoadBank);
        }
        static void LoadBank(LuaState L)
        {
            int nargs = LuaBindings.lua_gettop(L);
            var path = LuaBindings.luaL_tolstring(L, 1, IntPtr.Zero);
            var value = LuaBindings.lua_toboolean(L, 2) != 0;
            ExtenderAudioManager.LoadBank(path, value);
        }
        void SetHook()
        {

        }
        public void TestLog(LuaState L)
        {
            int nargs = LuaBindings.lua_gettop(L);
            for (int i = 1; i <= nargs; ++i)
            {
                var text = LuaBindings.luaL_tolstring(L, i, IntPtr.Zero);
                Console.Write(text);
            }
            Console.WriteLine();
        }

        public void Eval(string code)
        {
            try
            {
                lua.Eval(code);
            } catch(Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
            }
        }

        public void Update()
        {
            if (DebugEnabled)
            {
                var top = LuaBindings.lua_gettop(State);
                LuaBindings.lua_getglobal(State, "OnExtenderDebugUpdate");
                if (LuaBindingMacros.lua_isfunction(State, -1))
                {
                    var result = LuaBindings.lua_pcallk(State, 0, 0, 0, 0, IntPtr.Zero);
                } else
                {
                    LuaBindings.lua_settop(State, top);
                }
            }
        }

        public static string StackTrace()
        {
            if(luaInterface == null || luaInterface->state == IntPtr.Zero)
            {
                return "Lua is null";
            }
            if (luaInterface->destroyed)
            {
                return "Lua is destroyed";
            }
            var sb = new StringBuilder();
            LuaDebug info = new LuaDebug();
            int level = 0;
            while (LuaBindings.lua_getstack(State, level, ref info) != 0)
            {
                LuaBindings.lua_getinfo(State, "nSl", ref info);
                var short_src = Marshal.PtrToStringAnsi(new IntPtr(info.short_src));
                var line = string.Format("  [{0}] {1}:{2} -- {3} [{4}]",
                    level, short_src, info.currentline,
                    (info.name != null ? info.name : "<unknown>"),
                    info.what);
                sb.AppendLine(line);
                level++;
            }
            return sb.ToString();
        }

        public static bool LuaValid => luaInterface != null && luaInterface->destroyed == false && luaInterface->state != IntPtr.Zero;
    }
}
