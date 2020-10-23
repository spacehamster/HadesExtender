using Reloaded.Hooks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace HadesExtender
{
    public unsafe class ScriptManager
    {
        //TODO: Make config
        const bool CustomLuaRuntime = true;
        SymbolResolver Resolver;
        public Lua lua;
        delegate void LuaFunc(LuaState L);
        LuaInterface* luaInterface;
        public LuaState State => luaInterface->state;
        public ScriptManager(SymbolResolver resolver)
        {
            Console.WriteLine("Initializing ScriptManager");
            Resolver = resolver;
            luaInterface = resolver.Resolve<LuaInterface>("?LUA_INTERFACE@ScriptManager@sgg@@2ULua@@A");
            lua = new Lua(resolver, luaInterface);
            if (CustomLuaRuntime)
            {
                LoadCustomRuntime(resolver);
            }
        }
        static Dictionary<string, object> luahooks = new Dictionary<string, object>();
        static void LoadCustomRuntime(SymbolResolver resolver)
        {
            Kernel32.LoadLibrary(Path.Combine(Util.ExtenderDirectory, "Lua.dll"));
            var luaModule = Util.GetModule("Lua.dll");
            var luaResolver = new DiaSymbolResolver(luaModule);
            var patten = new Regex("lua*");

            foreach (var symbol in resolver.FindSymbolsMatching(patten))
            {
                if (!luaResolver.TryResolve(symbol, out var target))
                {
                    Console.WriteLine($"Could not find symbol {symbol} in lua.dll");
                    continue;
                }

                var source = resolver.Resolve(symbol);
                var asm = new string[] {
                    $"use64",
                    $"mov rax, {target.ToInt64()}",
                    $"jmp rax"
                };
                var hook = new AsmHook(asm, source.ToInt64(), Reloaded.Hooks.Definitions.Enums.AsmHookBehaviour.DoNotExecuteOriginal).Activate();
                luahooks[symbol] = hook;
                Console.WriteLine($"hooked lua function {symbol}");
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
                LuaHelper.OpenLibraries(State);
            }
            var debugDir = Environment.GetEnvironmentVariable("HadesExtenderDebugDirectory");
            if (debugDir != null)
            {
                Console.WriteLine("Loading debug scripts");
                var luadir = Path.Combine(debugDir, "lua_modules");
                lua.Eval(string.Format(@"package.path = package.path .. "";{0}""",
                    $@"{debugDir}\?.lua".Replace(@"\", @"\\")));
                lua.Eval(string.Format(@"package.path = package.path .. "";{0}""",
                    $@"{debugDir}\share\lua\5.2\?.lua".Replace(@"\", @"\\")));
                lua.Eval(string.Format(@"package.cpath = package.cpath .. "";{0}""",
                    $@"{luadir}\lib\lua\5.2\?.dll".Replace(@"\", @"\\")));
                if (File.Exists($"{debugDir}/Debug.lua")) lua.LoadFile($"{debugDir}/Debug.lua");
            }
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
            LuaBindings.lua_getglobal(State, "DebugUpdate");
            LuaBindings.lua_pcallk(State, 0, 0, 0, 0, IntPtr.Zero);
        }
    }
}
