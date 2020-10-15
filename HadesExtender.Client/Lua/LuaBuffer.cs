using System;

namespace HadesExtender
{
    public struct LuaBuffer
    {
        IntPtr Buffer;

        public LuaBuffer(IntPtr buffer)
        {
            Buffer = buffer;
        }

        public static implicit operator LuaBuffer(IntPtr ptr)
        {
            return new LuaBuffer(ptr);
        }

        public static implicit operator IntPtr(LuaBuffer buffer)
        {
            return buffer.Buffer;
        }
    }
}
