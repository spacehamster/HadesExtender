@echo off
title BuildAndCopyLibs

SET SolutionDir=%~dp0
SET SolutionDir=%SolutionDir:~0,-1%
SET Config=Debug
SET Bin=%SolutionDir%\x64\%Config%
SET CPATH=lua_modules\lib\lua\5.2
SET LPATH=lua_modules\share\lua\5.2

if "%~1" == "" (
 SET Config=Debug
) else (
 SET Config=%~1
)
echo Building and copying libs for %Config% config

msbuild %SolutionDir%\HadesExtender.sln /t:Lua;LuaHelper;LuaModules\cjson;LuaModules\lfs;LuaModules\mime;LuaModules\socket /p:Platform="x64" /p:Configuration=%Config% -m

call :copyFile %Bin%\Lua.dll Lua.dll || GOTO :error
call :copyFile %Bin%\LuaHelper.dll LuaHelper.dll || GOTO :error
call :copyFile %Bin%\cjson.dll %CPATH%\cjson.dll || GOTO :error
call :copyFile %Bin%\socket\core.dll %CPATH%\socket\core.dll || GOTO :error
call :copyFile %Bin%\mime\core.dll %CPATH%\mime\core.dll || GOTO :error
call :copyFile %Bin%\lfs.dll %CPATH%\lfs.dll || GOTO :error

call :copyFile %SolutionDir%\LuaModules\luasocket\src\socket.lua %LPATH%\socket.lua || GOTO :error
call :copyFile %SolutionDir%\LuaModules\luasocket\src\mime.lua %LPATH%\mime.lua || GOTO :error
call :copyFile %SolutionDir%\LuaModules\luasocket\src\ltn12.lua %LPATH%\ltn12.lua || GOTO :error
call :copyFile %SolutionDir%\LuaModules\luasocket\src\ftp.lua %LPATH%\socket\ftp.lua || GOTO :error
call :copyFile %SolutionDir%\LuaModules\luasocket\src\headers.lua %LPATH%\socket\headers.lua || GOTO :error
call :copyFile %SolutionDir%\LuaModules\luasocket\src\http.lua %LPATH%\socket\http.lua || GOTO :error
call :copyFile %SolutionDir%\LuaModules\luasocket\src\smtp.lua %LPATH%\socket\smtp.lua || GOTO :error
call :copyFile %SolutionDir%\LuaModules\luasocket\src\tp.lua %LPATH%\socket\tp.lua || GOTO :error
call :copyFile %SolutionDir%\LuaModules\luasocket\src\url.lua %LPATH%\socket\url.lua || GOTO :error

call :copyFile %SolutionDir%\LuaScripts\InitDebugging.lua InitDebugging.lua || GOTO :error
call :copyFile %SolutionDir%\LuaScripts\vscode-debuggee.lua vscode-debuggee.lua || GOTO :error

echo Success
EXIT /B %ERRORLEVEL%

:error
ECHO Failed with error #%errorlevel%.
EXIT /b %errorlevel%


:copyFile
echo Copying %~nx1
if not exist %~1 (
	echo Error: File %~1 not found
	EXIT /B 1
)
if not exist %~dp2 (
	mkdir %~dp2
)
copy %~1 %~2 > NUL
EXIT /B %ERRORLEVEL%