using System;

namespace HadesExtender
{
    public struct LuaState
    {
        IntPtr State;

        public LuaState(IntPtr state)
        {
            State = state;
        }

        public static implicit operator LuaState(IntPtr ptr)
        {
            return new LuaState(ptr);
        }

        public static implicit operator IntPtr(LuaState state)
        {
            return state.State;
        }
    }
}
