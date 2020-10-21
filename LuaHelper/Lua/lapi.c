/*
** $Id: lapi.c,v 2.171.1.1 2013/04/12 18:48:47 roberto Exp $
** Lua API
** See Copyright Notice in lua.h
*/


#include <stdarg.h>
#include <string.h>

#define lapi_c
#define LUA_CORE

#include "lua.h"

#include "lapi.h"
#include "ldebug.h"
#include "ldo.h"
#include "lfunc.h"
#include "lgc.h"
#include "lmem.h"
#include "lobject.h"
#include "lstate.h"
#include "lstring.h"
#include "ltable.h"
#include "ltm.h"
#include "lundump.h"
#include "lvm.h"

LUA_API void lua_setallocf(lua_State* L, lua_Alloc f, void* ud) {
    lua_lock(L);
    G(L)->ud = ud;
    G(L)->frealloc = f;
    lua_unlock(L);
}