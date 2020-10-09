using System;

namespace HadesExtender
{
    public struct LuaInterface
    {
        public IntPtr state;
        public int msghander;
        public bool destroyed;
    }
}
