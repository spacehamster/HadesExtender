using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;

namespace HadesExtender
{
    public unsafe class ScriptManager
    {
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
        }

        public void Init()
        {
            Console.WriteLine("Registered TestLog function");
            lua.RegisterFunction<LuaFunc>("TestLog", TestLog);
            Console.WriteLine("Registered TesValue global");
            lua.SetGlobal("TestValue", (double)5);

            LuaHelper.OpenLibraries(State);
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
