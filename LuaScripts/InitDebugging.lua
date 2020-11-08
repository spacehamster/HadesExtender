local json = require 'cjson'
local debuggee = require 'vscode-debuggee'
local lfs = require 'lfs'
local filepathCache = {}

local function BuildLookup(root, directory)
	local currentDir = root .. "/" .. directory
	for file in lfs.dir(currentDir) do
		local filepath = currentDir .. '/' .. file
		if file == "." or file == ".." then
			--skip
		elseif lfs.attributes(filepath, "mode") == "file" then
			local chunkname = file:match("(.+)%..+")
			if chunkname ~= nil and filepathCache[chunkname] == nil then
				filepathCache[chunkname] = directory .. "/" .. file
			end
		elseif lfs.attributes(filepath, "mode") == "directory" then
			BuildLookup(root, directory .. "/" .. file)
		end
	end
end

local function ResolveChunkPath(filename)
	if filepathCache[filename] ~= nil then
		return filepathCache[filename]
	else
		return filename
	end
end

BuildLookup("..", "Content/Scripts")
BuildLookup("..", "Content/Mods")
local startResult, breakerType = debuggee.start(json, { resolveChunkPath = ResolveChunkPath })
print('debuggee start ->', startResult, breakerType)

function OnExtenderDebugUpdate()
	debuggee.poll()
end