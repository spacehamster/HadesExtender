local json = require 'cjson'
local debuggee = require 'vscode-debuggee'

function ResolveChunkPath(fileName)
	return "Content/Scripts/" .. fileName .. ".lua"
end

local startResult, breakerType = debuggee.start(json, { resolveChunkPath = ResolveChunkPath })
print('debuggee start ->', startResult, breakerType)

function OnExtenderDebugUpdate()
	debuggee.poll()
end

function abc()
	DebugWriteLine("ABC", "DEF")
end