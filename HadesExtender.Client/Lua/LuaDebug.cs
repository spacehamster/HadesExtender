using System.Runtime.InteropServices;

namespace HadesExtender
{
    public unsafe struct LuaDebug
    {
        public const int LUA_IDSIZE = 60;
        public int @event;
        [MarshalAs(UnmanagedType.LPStr)] public string name;
        [MarshalAs(UnmanagedType.LPStr)] public string namewhat;
        [MarshalAs(UnmanagedType.LPStr)] public string what;
        [MarshalAs(UnmanagedType.LPStr)] public string source;
        public int currentline;
        public int linedefined;
        public int lastlinedefined;
        public byte nups;
        public byte nparams;
        public sbyte isvararg;
        public sbyte istailcall;
        public fixed sbyte short_src[LUA_IDSIZE];
        public void* i_ci;
    }
}
