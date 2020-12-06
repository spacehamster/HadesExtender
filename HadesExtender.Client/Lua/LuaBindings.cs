using System;
using System.Runtime.InteropServices;

namespace HadesExtender
{
    public static class LuaBindings
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int LuaAbsIndexDelegate(IntPtr L, int idx);
        [PdbSymbol]
        public static LuaAbsIndexDelegate lua_absindex = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate CFunction LuaAtPanicDelegate(LuaState L, CFunction panicf);
        [PdbSymbol]
        public static LuaAtPanicDelegate lua_atpanic = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LuaCallKDelegate(LuaState L, int nargs, int nresults, int ctx, CFunction k);
        [PdbSymbol]
        public static LuaCallKDelegate lua_callk = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int LuaCheckStackDelegate(LuaState L, int extra);
        [PdbSymbol]
        public static LuaCheckStackDelegate lua_checkstack = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LuaCloseDelegate(LuaState L);
        [PdbSymbol]
        public static LuaCloseDelegate lua_close = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int LuaCompareDelegate(LuaState L, int index1, int index2, int op);
        [PdbSymbol]
        public static LuaCompareDelegate lua_compare = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LuaConcatDelegate(LuaState L, int n);
        [PdbSymbol]
        public static LuaConcatDelegate lua_concat = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LuaCopyDelegate(LuaState L, int fromidx, int toidx);
        [PdbSymbol]
        public static LuaCopyDelegate lua_copy = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LuaCreateTableDelegate(LuaState L, int narr, int nrec);
        [PdbSymbol]
        public static LuaCreateTableDelegate lua_createtable = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int LuaErrorDelegate(LuaState L);
        [PdbSymbol]
        public static LuaErrorDelegate lua_error = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int LuaGCDelegate(LuaState L, int what, int data);
        [PdbSymbol]
        public static LuaGCDelegate lua_gc = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate CFunction LuaGetAllocFDelegate(LuaState L, int what, IntPtr ud);
        [PdbSymbol]
        public static LuaGetAllocFDelegate lua_getallocf = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LuaGetFieldDelegate(LuaState L, int index, [MarshalAs(UnmanagedType.LPStr)] string k);
        [PdbSymbol]
        public static LuaGetFieldDelegate lua_getfield = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LuaGetGlobalDelegate(LuaState L, [MarshalAs(UnmanagedType.LPStr)] string name);
        [PdbSymbol]
        public static LuaGetGlobalDelegate lua_getglobal = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate CFunction LuaGetHookDelegate(LuaState L);
        [PdbSymbol]
        public static LuaGetHookDelegate lua_gethook = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int LuaGetHookCount(LuaState L);
        [PdbSymbol]
        public static LuaGetHookCount lua_gethookcount = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int LuaGetHookMask(LuaState L);
        [PdbSymbol]
        public static LuaGetHookMask lua_gethookmask = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int LuaGetInfoDelegate(LuaState L, [MarshalAs(UnmanagedType.LPStr)] string what, ref LuaDebug ar);
        [PdbSymbol]
        public static LuaGetInfoDelegate lua_getinfo = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [return:MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(LStringMarshaler))]
        public delegate string LuaGetLocalDelegate(LuaState L,IntPtr ar, int n);
        [PdbSymbol]
        public static LuaGetLocalDelegate lua_getlocal = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LuaGetMetaTable(LuaState L, [MarshalAs(UnmanagedType.LPStr)] string tname);
        [PdbSymbol]
        public static LuaGetMetaTable lua_getmetatable = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int LuaGetStack(LuaState L, int level, ref LuaDebug ar);
        [PdbSymbol]
        public static LuaGetStack lua_getstack = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LuaGetTableDelegate(LuaState L, int index);
        [PdbSymbol]
        public static LuaGetTableDelegate lua_gettable = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int LueGetTopDelegate(LuaState L);
        [PdbSymbol]
        public static LueGetTopDelegate lua_gettop = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [return:MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(LStringMarshaler))]
        public delegate string LuaGetUpValue(LuaState L, int funcindex, int n);
        [PdbSymbol]
        public static LuaGetUpValue lua_getupvalue = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LuaGetUserValueDelegate(LuaState L, int index);
        [PdbSymbol]
        public static LuaGetUserValueDelegate lua_getuservalue = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LuaInsertDelegate(LuaState L, int index);
        [PdbSymbol]
        public static LuaInsertDelegate lua_insert = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int LuaIsCFunction(LuaState L, int index);
        [PdbSymbol]
        public static LuaIsCFunction lua_iscfunction = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int LuaIsNumberDelegate(LuaState L, int index);
        [PdbSymbol]
        public static LuaIsNumberDelegate lua_isnumber = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int LuaIsStringDelgate(LuaState L, int index);
        [PdbSymbol]
        public static LuaIsStringDelgate lua_isstring = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int LuaLoadDelegate(LuaState L, 
            CFunction reader, 
            IntPtr data, 
            [MarshalAs(UnmanagedType.LPStr)] string source,
            [MarshalAs(UnmanagedType.LPStr)] string mode);
        [PdbSymbol]
        public static LuaLoadDelegate lua_load = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate LuaState LuaNewStateDelegate(CFunction f, IntPtr ud);
        [PdbSymbol]
        public static LuaNewStateDelegate lua_newstate = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate LuaState LuaNewThreadDelegate(LuaState L);
        [PdbSymbol]
        public static LuaNewThreadDelegate lua_newthread = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int LuaNextDelegate(LuaState L, int index);
        [PdbSymbol]
        public static LuaNextDelegate lua_next = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate ResultCode LuaPCallKDelegate(LuaState L,
            int nargs,
            int nresults,
            int errfunc,
            int ctx,
            CFunction k);
        [PdbSymbol]
        public static LuaPCallKDelegate lua_pcallk = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LuaPushBooleanDelegate(LuaState L, int b);
        [PdbSymbol]
        public static LuaPushBooleanDelegate lua_pushboolean = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LuaPushCClosureDelegate(LuaState L, CFunction fn, int n);
        [PdbSymbol]
        public static LuaPushCClosureDelegate lua_pushcclosure = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LuaPushFStringDelegate(LuaState L, 
            [MarshalAs(UnmanagedType.LPStr)] string fmt, 
            IntPtr args = default);
        [PdbSymbol]
        public static LuaPushFStringDelegate lua_pushfstring = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LuaPushIntegerDelegate(LuaState L, LuaInteger n);
        [PdbSymbol]
        public static LuaPushIntegerDelegate lua_pushinteger = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LuaPushLightUserDataDelegate(LuaState L, IntPtr p);
        [PdbSymbol]
        public static LuaPushLightUserDataDelegate lua_pushlightuserdata = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LuaPushLStringDelegate(LuaState L, [MarshalAs(UnmanagedType.LPStr)] string s, LuaInteger n);
        [PdbSymbol]
        public static LuaPushLStringDelegate lua_pushlstring = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LuaPushNilDelegate(LuaState L);
        [PdbSymbol]
        public static LuaPushNilDelegate lua_pushnil = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LuaPushNumberDelegate(LuaState L, double n);
        [PdbSymbol]
        public static LuaPushNumberDelegate lua_pushnumber = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LuaPushStringDelegate(LuaState L, [MarshalAs(UnmanagedType.LPStr)] string s);
        [PdbSymbol]
        public static LuaPushStringDelegate lua_pushstring = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int LuaPushThreadDelegate(LuaState L);
        [PdbSymbol]
        public static LuaPushThreadDelegate lua_pushthread = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LuaPushValueDelegate(LuaState L, int index);
        [PdbSymbol]
        public static LuaPushValueDelegate lua_pushvalue = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LuaPushVFStringDelegate(LuaState L, 
            [MarshalAs(UnmanagedType.LPStr)] string fmt,
            IntPtr argp);
        [PdbSymbol]
        public static LuaPushVFStringDelegate lua_pushvfstring = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int LuaRawEqualDelegate(LuaState L, int index1, int index2);
        [PdbSymbol]
        public static LuaRawEqualDelegate lua_rawequal = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LuaRawGetDelegate(LuaState L, int index);
        [PdbSymbol]
        public static LuaRawGetDelegate lua_rawget = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LuaRawGetIDelegate(LuaState L, int index, int n);
        [PdbSymbol]
        public static LuaRawGetIDelegate lua_rawgeti = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate LuaInteger LuaRawLen(LuaState L, int index);
        [PdbSymbol]
        public static LuaRawLen lua_rawlen = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LuaRawSetDelegate(LuaState L, int index);
        [PdbSymbol]
        public static LuaRawSetDelegate lua_rawset = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LuaRawSetIDelegate(LuaState L, int index, int n);
        [PdbSymbol]
        public static LuaRawSetIDelegate lua_rawseti = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LuaRemoveDelgate(LuaState L, int index);
        [PdbSymbol]
        public static LuaRemoveDelgate lua_remove = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LuaReplaceDelegate(LuaState L, int index);
        [PdbSymbol]
        public static LuaReplaceDelegate lua_replace = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LuaResumeDelegate(LuaState L, int index);
        [PdbSymbol]
        public static LuaResumeDelegate lua_resume = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LuaSetFieldDelegate(LuaState L, int index, [MarshalAs(UnmanagedType.LPStr)] string k);
        [PdbSymbol]
        public static LuaSetFieldDelegate lua_setfield = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LuaSetGlobalDelegate(LuaState L, [MarshalAs(UnmanagedType.LPStr)] string name);
        [PdbSymbol]
        public static LuaSetGlobalDelegate lua_setglobal = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LuaSetHookDelegate(LuaState L, CFunction f, int mask, int count);
        [PdbSymbol]
        public static LuaSetHookDelegate lua_sethook = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [return:MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(LStringMarshaler))]
        public delegate string LuaSetLocalDelegate(LuaState L, IntPtr ar, int n);
        [PdbSymbol]
        public static LuaSetLocalDelegate lua_setlocal = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LuaSetMetaTableDelegate(LuaState L, [MarshalAs(UnmanagedType.LPStr)] string tname);
        [PdbSymbol]
        public static LuaSetMetaTableDelegate lua_setmetatable = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LuaSetTableDelegate(LuaState L, int index);
        [PdbSymbol]
        public static LuaSetTableDelegate lua_settable = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LuaSetTopDelegate(LuaState L, int index);
        [PdbSymbol]
        public static LuaSetTopDelegate lua_settop = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LuaSetUpValue(LuaState L, int funcindex, int n);
        [PdbSymbol]
        public static LuaSetUpValue lua_setupvalue = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LuaSetUserValueDelegate(LuaState L, int index);
        [PdbSymbol]
        public static LuaSetUserValueDelegate lua_setuservalue = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int LuaStatusDelegate(LuaState L);
        [PdbSymbol]
        public static LuaStatusDelegate lua_status = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int LuaToBooleanDelegate(LuaState L, int index);
        [PdbSymbol]
        public static LuaToBooleanDelegate lua_toboolean = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int LuaToIntegerXDelegate (LuaState L, int index, out int isnum);
        [PdbSymbol]
        public static LuaToIntegerXDelegate lua_tointegerx = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [return:MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(LStringMarshaler))]
        public delegate string LuaToLStringDelegate(LuaState L, int index, IntPtr len);
        [PdbSymbol]
        public static LuaToLStringDelegate lua_tolstring = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate double LuaToNumberXDelegate(LuaState L, int index, out int isnum);
        [PdbSymbol]
        public static LuaToNumberXDelegate lua_tonumberx = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr LuaToPointerDelegate(LuaState L, int index);
        [PdbSymbol]
        public static LuaToPointerDelegate lua_topointer = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate LuaState LuaToThreadDelegate(LuaState L, int index);
        [PdbSymbol]
        public static LuaToThreadDelegate lua_tothread = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate uint LuaToUnsignedXDelegate(LuaState L, int index, out int isnum);
        [PdbSymbol]
        public static LuaToUnsignedXDelegate lua_tounsignedx = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr LuaToUserDataDelegate(LuaState L, int index);
        [PdbSymbol]
        public static LuaToUserDataDelegate lua_touserdata = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate LuaType LuaTypeDelegate(LuaState L, int index);
        [PdbSymbol]
        public static LuaTypeDelegate lua_type = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [return:MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(LStringMarshaler))]
        public delegate string LuaTypeNameDelegate(LuaState L, int tp);
        [PdbSymbol]
        public static LuaTypeNameDelegate lua_typename = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr LuaUpValueIdDelegate(LuaState L, int funcindex, int n);
        [PdbSymbol]
        public static LuaUpValueIdDelegate lua_upvalueid = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LuaUpValueJoinDelegate(LuaState L, int funcindex1, int n1, int funcindex2, int n2);
        [PdbSymbol]
        public static LuaUpValueJoinDelegate lua_upvaluejoin = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate double* LuaVersionDelegate(LuaState L);
        [PdbSymbol]
        public static LuaVersionDelegate lua_version = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void LuaXMoveDelegate(LuaState from, LuaState to, int n);
        [PdbSymbol]
        public static LuaXMoveDelegate lua_xmove = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void LuaLAddLStringDelegate(LuaBuffer B, IntPtr s, LuaInteger size);
        [PdbSymbol]
        public static LuaLAddLStringDelegate luaL_addlstring = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void LuaLAddStringDelegate(LuaBuffer B, [MarshalAs(UnmanagedType.LPStr)] string s);
        [PdbSymbol]
        public static LuaLAddStringDelegate luaL_addstring = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void LuaLAddValueDelegate(LuaBuffer B);
        [PdbSymbol]
        public static LuaLAddValueDelegate luaL_addvalue = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate int LuaLArgErrorDelegate(LuaState L, int arg, [MarshalAs(UnmanagedType.LPStr)] string extramsg);
        [PdbSymbol]
        public static LuaLArgErrorDelegate luaL_argerror = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void LuaLBuffInitDelegate(LuaState L, LuaBuffer B);
        [PdbSymbol]
        public static LuaLBuffInitDelegate luaL_buffinit = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public unsafe delegate string LuaLBuffInitSizeDelegate(LuaState L, LuaBuffer B, LuaInteger sz);
        [PdbSymbol]
        public static LuaLBuffInitSizeDelegate luaL_buffinitsize = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate int LuaLCallMetaDelegate(LuaState L, int obj, [MarshalAs(UnmanagedType.LPStr)] string e);
        [PdbSymbol]
        public static LuaLCallMetaDelegate luaL_callmeta = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void LuaLCheckAnyDelegate(LuaState L, int arg);
        [PdbSymbol]
        public static LuaLCheckAnyDelegate luaL_checkany = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate int LuaLCheckIntegerDelegate(LuaState L, int arg);
        [PdbSymbol]
        public static LuaLCheckIntegerDelegate luaL_checkinteger = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [return:MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(LStringMarshaler))]
        public unsafe delegate string LuaLCheckLStringDelegate(LuaState L, int arg, IntPtr l);
        [PdbSymbol]
        public static LuaLCheckLStringDelegate luaL_checklstring = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate double LuaLCheckNumberDelegate(LuaState L, int arg);
        [PdbSymbol]
        public static LuaLCheckNumberDelegate luaL_checknumber = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate int LuaLCheckOptionDelegate(LuaState L, 
                int arg,
                [MarshalAs(UnmanagedType.LPStr)] string def,
                [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] string[] list
                );
        [PdbSymbol]
        public static LuaLCheckOptionDelegate luaL_checkoption = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void LuaLCheckStackDelegate(LuaState L, int sz, [MarshalAs(UnmanagedType.LPStr)] string msg);
        [PdbSymbol]
        public static LuaLCheckStackDelegate luaL_checkstack = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void LuaLCheckTypeDelegate(LuaState L, int arg, int t);
        [PdbSymbol]
        public static LuaLCheckTypeDelegate luaL_checktype = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate uint LuaLCheckUnsignedDelegate(LuaState L, int arg);
        [PdbSymbol]
        public static LuaLCheckUnsignedDelegate luaL_checkunsigned = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate int LuaLErrorDelegate(LuaState L, [MarshalAs(UnmanagedType.LPStr)] string fmt, IntPtr vargs);
        [PdbSymbol]
        public static LuaLErrorDelegate luaL_error = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate int LuaLGetMetaFieldDelegate(LuaState L, int obj, [MarshalAs(UnmanagedType.LPStr)] string e);
        [PdbSymbol]
        public static LuaLGetMetaFieldDelegate luaL_getmetafield = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate int LuaLGetSubtableDelegate(LuaState L, int index, [MarshalAs(UnmanagedType.LPStr)] string fname);
        [PdbSymbol]
        public static LuaLGetSubtableDelegate luaL_getsubtable = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate int LuaLLenDelegate(LuaState L, int index);
        [PdbSymbol]
        public static LuaLLenDelegate luaL_len = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate ResultCode LuaLLoadBufferXDelegate(LuaState L, 
            [MarshalAs(UnmanagedType.LPStr)] string buff, 
            LuaInteger sz,
            [MarshalAs(UnmanagedType.LPStr)] string name,
            [MarshalAs(UnmanagedType.LPStr)] string mode);
        [PdbSymbol]
        public static LuaLLoadBufferXDelegate luaL_loadbufferx = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate int LuaLLoadFileXDelegate(LuaState L, 
            [MarshalAs(UnmanagedType.LPStr)] string filename, 
            [MarshalAs(UnmanagedType.LPStr)] string mode);
        [PdbSymbol]
        public static LuaLLoadFileXDelegate luaL_loadfilex = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void LuaLOpenLibsDelegate(LuaState L);
        [PdbSymbol]
        public static LuaLOpenLibsDelegate luaL_openlibs = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate LuaInteger LuaLOptIntegerDelegate(LuaState L, int arg, LuaInteger d);
        [PdbSymbol]
        public static LuaLOptIntegerDelegate luaL_optinteger = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate IntPtr LuaLOptLStringDelegate(LuaState L, int arg, IntPtr d, IntPtr l);
        [PdbSymbol]
        public static LuaLOptLStringDelegate luaL_optlstring = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public unsafe delegate string LuaLPrepBufferSizeDelegate(LuaBuffer B, LuaInteger sz);
        [PdbSymbol]
        public static LuaLPrepBufferSizeDelegate luaL_prepbuffsize = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void LuaLPushResultDelegate(LuaBuffer B);
        [PdbSymbol]
        public static LuaLPushResultDelegate luaL_pushresult = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void LuaLPushResultSizeDelegate(LuaBuffer B, LuaInteger sz);
        [PdbSymbol]
        public static LuaLPushResultSizeDelegate luaL_pushresultsize = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate int LuaLRefDelegate(LuaState L, int t);
        [PdbSymbol]
        public static LuaLRefDelegate luaL_ref = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void LuaLRequireFDelegate(LuaState L, 
            [MarshalAs(UnmanagedType.LPStr)] string modname,
            CFunction openf,
            int glb);
        [PdbSymbol]
        public static LuaLRequireFDelegate luaL_requiref = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void LuaLSetFuncsDelegate(LuaState L, IntPtr l, int nup);
        [PdbSymbol]
        public static LuaLSetFuncsDelegate luaL_setfuncs = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [return:MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(LStringMarshaler))]
        public unsafe delegate string LuaLToLStringDelegate(LuaState L, int index, IntPtr len);
        [PdbSymbol]
        public static LuaLToLStringDelegate luaL_tolstring = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void LuaLTracebackDelegate(LuaState L, 
            LuaState L1, 
            [MarshalAs(UnmanagedType.LPStr)] string msg,
            int level);
        [PdbSymbol]
        public static LuaLTracebackDelegate luaL_traceback = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void LuaLUnrefDelegate(LuaState L, int t, int _ref);
        [PdbSymbol]
        public static LuaLUnrefDelegate luaL_unref = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void LuaLWhereDelegate(LuaState L, int lvl);
        [PdbSymbol]
        public static LuaLWhereDelegate luaL_where = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate int LuaOpenDebugDelegate(LuaState L);
        [PdbSymbol]
        public static LuaOpenDebugDelegate luaopen_debug = null;
    }
}
