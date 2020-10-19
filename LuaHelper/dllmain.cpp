// dllmain.cpp : Defines the entry point for the DLL application.
#define WIN32_LEAN_AND_MEAN             // Exclude rarely-used stuff from Windows headers
// Windows Header Files
#include <windows.h>
#include <map>
#include <string>
#include <stdio.h>
#define EXPORT __declspec(dllexport)
void load_functions(std::map<std::string, FARPROC> lookup);
extern "C" {
    EXPORT void InitProxy(char** names, FARPROC* addresses, int count) {
        std::map<std::string, FARPROC> lookup;
        for (int i = 0; i < count; i++) {
            lookup[names[i]] = addresses[i];
        }
        load_functions(lookup);
    }
}