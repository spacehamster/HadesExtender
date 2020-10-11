using System;
using System.Runtime.InteropServices;
using static HadesExtender.LuaBindings;
namespace HadesExtender
{
    public unsafe class Lua
    {
        LuaInterface* LuaState;

        public Lua(SymbolResolver resolver, LuaInterface* luaState)
        {
            LuaState = luaState;
        }

        public void RegisterFunction<T>(string name, T method) where T : Delegate
        {
            if (LuaState == null || LuaState->state == IntPtr.Zero) throw new Exception("LuaState is null");
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
    }
}
