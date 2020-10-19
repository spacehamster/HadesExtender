namespace HadesExtender
{
    public static class LuaBindingMacros
    {
        public static void lua_pop(LuaState L, int n)
        {
            LuaBindings.lua_settop(L, -(n) - 1);
        }
    }
}
