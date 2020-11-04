using static HadesExtender.LuaBindings;
namespace HadesExtender
{
    public static class LuaBindingMacros
    {
        public static void lua_pop(LuaState L, int n)
        {
            lua_settop(L, -(n) - 1);
        }
        public static bool lua_isfunction(LuaState L, int n)
        {
            return lua_type(L, n) == LuaType.Function;
        }
        public static bool lua_istable(LuaState L, int n)
        {
            return lua_type(L, n) == LuaType.Table;
        }
        public static bool lua_islightuserdata(LuaState L, int n)
        {
            return lua_type(L, n) == LuaType.LightUserData;
        }
        public static bool lua_isnil(LuaState L, int n)
        {
            return lua_type(L, n) == LuaType.Nil;
        }
        public static bool lua_isboolean(LuaState L, int n)
        {
            return lua_type(L, n) == LuaType.Boolean;
        }
        public static bool lua_isthread(LuaState L, int n)
        {
            return lua_type(L, n) == LuaType.Thread;
        }
        public static bool lua_isnone(LuaState L, int n)
        {
            return lua_type(L, n) == LuaType.None;
        }
        public static bool lua_isnoneornil(LuaState L, int n)
        {
            return lua_type(L, n) <= 0;
        }
    }
}
