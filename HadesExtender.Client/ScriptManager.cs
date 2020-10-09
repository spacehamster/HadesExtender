using System;
using System.Runtime.InteropServices;

namespace HadesExtender
{
    public unsafe class ScriptManager
    {
        SymbolResolver Resolver;
        Lua lua;

        public ScriptManager(SymbolResolver resolver)
        {
            Console.WriteLine("Initializing ScriptManager");
            Resolver = resolver;
            var luaInterface = resolver.Resolve<LuaInterface>("?LUA_INTERFACE@ScriptManager@sgg@@2ULua@@A");
            lua = new Lua(resolver, luaInterface);
        }

        public void Init()
        {
            Console.WriteLine("Registered TestLog function");
            lua.RegisterFunction<Action>("TestLog", TestLog);
            Console.WriteLine("Registered TesValue global");
            lua.SetGlobal("TestValue", (double)5);

        }
        public void TestLog()
        {
            Console.WriteLine("TestLog triggered");
        }
    }
}
