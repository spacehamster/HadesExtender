using System;
using System.Runtime.InteropServices;

namespace HadesExtender
{
    public unsafe class Lua
    {
        LuaInterface* LuaState;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate int LuaGetTopDelegate(IntPtr luaState);
        [PdbSymbol]
        static LuaGetTopDelegate lua_gettop = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void LuaSetTopDelegate(IntPtr luaState, int param);
        [PdbSymbol]
        static LuaSetTopDelegate lua_settop = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void LuaPushLightUserDataDelegate(IntPtr luaState, IntPtr param);
        [PdbSymbol]
        static LuaPushLightUserDataDelegate lua_pushlightuserdata = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void LuaPushCClosureDelegate(IntPtr luaState, IntPtr func, int parameterCount);
        [PdbSymbol]
        static LuaPushCClosureDelegate lua_pushcclosure = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void LusSetGlobalDelegate(IntPtr luaState, [MarshalAs(UnmanagedType.LPStr)] string param);
        [PdbSymbol]
        static LusSetGlobalDelegate lua_setglobal = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void LuaPushNumberDelegate(IntPtr luaState, double param);
        [PdbSymbol]
        static LuaPushNumberDelegate lua_pushnumber = null;

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
