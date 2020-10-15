using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using static HadesExtender.LuaBindings;
namespace HadesExtender
{
    public unsafe class Lua
    {
        const int LuaMultiRet = -1;
        LuaInterface* LuaState;
        Dictionary<string, Delegate> Functions = new Dictionary<string, Delegate>();
        public Lua(SymbolResolver resolver, LuaInterface* luaState)
        {
            LuaState = luaState;
        }

        public void RegisterFunction<T>(string name, T method) where T : Delegate
        {
            if (LuaState == null || LuaState->state == IntPtr.Zero) throw new Exception("LuaState is null");
            //add to dictionary to prevent delegate from being GC'd (TODO: confirm that this is a real concern)
            Functions[name] = method;
            var newTop = lua_gettop(LuaState->state);
            var pointer = Marshal.GetFunctionPointerForDelegate(method);
            lua_pushcclosure(LuaState->state, pointer, 0);
            lua_setglobal(LuaState->state, name);
            lua_settop(LuaState->state, newTop);
        }

        public void SetGlobal(string path, object obj)
        {
            if (LuaState == null || LuaState->state == IntPtr.Zero) throw new Exception("LuaState is null");
            if (obj is double d)
            {
                lua_pushnumber(LuaState->state, d);
                lua_setglobal(LuaState->state, path);
            }
            else
            {
                throw new NotImplementedException($"SetGlobal of type {obj.GetType().Name} not implemented");
            }
        }

        public void Eval(string code)
        {
            if (LuaState == null || LuaState->state == IntPtr.Zero) throw new Exception("LuaState is null");
            var result = luaL_loadbufferx(LuaState->state, code, code.Length, "REPL", null);
            if (result == ResultCode.OK)
            {
                result = lua_pcallk(LuaState->state, 0, LuaMultiRet, 0, 0, IntPtr.Zero);
                if (result != ResultCode.OK)
                {
                    PrintError(); // pcall failed
                }
            }
            else
            {
                PrintError(); // loadbuffer failed
            }
        }

       void PrintError()
        {
            Console.Error.WriteLine($"Error: {0}",
                luaL_checklstring(LuaState->state, -1, IntPtr.Zero));
        }
    }
}
