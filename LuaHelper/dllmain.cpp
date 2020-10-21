// dllmain.cpp : Defines the entry point for the DLL application.
#define WIN32_LEAN_AND_MEAN             // Exclude rarely-used stuff from Windows headers
// Windows Header Files
#include <windows.h>
#include <map>
#include <string>
#include <stdio.h>
#define EXPORT __declspec(dllexport)
/*
LuaHelper provides a proxy for lua api calls to the hades engine so libraries can link against export symbols and call into the lua runtime.
Hades has a number of  api functions that have been stripped from the engine, they have been reimplemented and call into the Lua runtime.
A number of stripped functions make use of the lua VM directly and Lua cannot handle multiple VMs, those functions would require certain VM api calls to be exposed,
so they are currently left unexported until a need for them arises. The stripped functions with a dependency on VM calls are:
lua_arith
lua_isuserdata
lua_rawgetp
lua_rawsetp
lua_tocfunction
*/
extern void load_functions(std::map<std::string, FARPROC> lookup);
extern "C" {
    EXPORT void InitProxy(char** names, FARPROC* addresses, int count) {
        std::map<std::string, FARPROC> lookup;
        for (int i = 0; i < count; i++) {
            lookup[names[i]] = addresses[i];
        }
        load_functions(lookup);
    }
}
