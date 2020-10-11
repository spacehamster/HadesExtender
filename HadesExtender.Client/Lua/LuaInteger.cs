using System;

namespace HadesExtender
{
    /*
     * Lua integers are defined as a ptrdiff_t, which is generally the same size as an IntPtr
     */
    public struct LuaInteger
    {
        IntPtr Value;

        public LuaInteger(IntPtr value)
        {
            Value = value;
        }

        public static implicit operator LuaInteger(long value)
        {
            return new LuaInteger(new IntPtr(value));
        }

        public static implicit operator long(LuaInteger luaInt)
        {
            return luaInt.Value.ToInt64();
        }
    }
}
