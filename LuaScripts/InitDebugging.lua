local json = require 'cjson'
local debuggee = require 'vscode-debuggee'

function ResolveChunkPath(fileName)
	return "Content/Scripts/" .. fileName .. ".lua"
end

local startResult, breakerType = debuggee.start(json, { resolveChunkPath = ResolveChunkPath })


function OnExtenderDebugUpdate()
	debuggee.poll()
end

function abc()
	DebugWriteLine("ABC", "DEF")
end