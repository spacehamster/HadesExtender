using System;
using System.Runtime.InteropServices;

namespace HadesExtender
{
    public unsafe class Lua
    {
        LuaInterface* LuaState;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate int LuaGetTopDelegate(IntPtr luaState);
        static LuaGetTopDelegate s_LuaGetTop;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void LuaSetTopDelegate(IntPtr luaState, int param);
        static LuaSetTopDelegate s_LuaSetTop;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void LuaPushLightUserDataDelegate(IntPtr luaState, IntPtr param);
        static LuaPushLightUserDataDelegate s_LuaPushLightUserData;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void LuaPushCClosureDelegate(IntPtr luaState, IntPtr func, int parameterCount);
        static LuaPushCClosureDelegate s_LuaPushCClosure;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void LusSetGlobalDelegate(IntPtr luaState, [MarshalAs(UnmanagedType.LPStr)] string param);
        static LusSetGlobalDelegate s_LuaSetGlobal;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void LuaPushNumberDelegate(IntPtr luaState, double param);
        static LuaPushNumberDelegate s_LuaPushNumber;

        public Lua(SymbolResolver resolver, LuaInterface* luaState)
        {
            LuaState = luaState;

            s_LuaGetTop = resolver.ResolveFunction<LuaGetTopDelegate>("lua_gettop");
            s_LuaSetTop = resolver.ResolveFunction<LuaSetTopDelegate>("lua_settop");
            s_LuaPushLightUserData = resolver.ResolveFunction<LuaPushLightUserDataDelegate>("lua_pushlightuserdata");
            s_LuaPushCClosure = resolver.ResolveFunction<LuaPushCClosureDelegate>("lua_pushcclosure");
            s_LuaSetGlobal = resolver.ResolveFunction<LusSetGlobalDelegate>("lua_setglobal");
            s_LuaPushNumber = resolver.ResolveFunction<LuaPushNumberDelegate>("lua_pushnumber");
        }

        public void RegisterFunction<T>(string name, T method) where T : Delegate
        {
            if (LuaState == null || LuaState->state == IntPtr.Zero) throw new Exception("LuaState is null");
            var newTop = s_LuaGetTop(LuaState->state);
            var pointer = Marshal.GetFunctionPointerForDelegate(method);
            s_LuaPushCClosure(LuaState->state, pointer, 0);
            s_LuaSetGlobal(LuaState->state, name);
            s_LuaSetTop(LuaState->state, newTop);
        }

        public void SetGlobal(string path, object obj)
        {
            if (LuaState == null || LuaState->state == IntPtr.Zero) throw new Exception("LuaState is null");
            if (obj is double d)
            {
                s_LuaPushNumber(LuaState->state, d);
                s_LuaSetGlobal(LuaState->state, path);
            }
            else
            {
                throw new NotImplementedException($"SetGlobal of type {obj.GetType().Name} not implemented");
            }
        }
    }
}
