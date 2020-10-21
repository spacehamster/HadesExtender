import argparse
import os
import re

def merge_tokens(template, tokens):
    repl = lambda matchobj : tokens[matchobj.group(1)]
    return re.sub(r"\$(\w+)\$", repl, template)
def parse_def_file(defFilePath):
    is_proxy_section = False
    result = []
    with open(defFilePath, "r", encoding="utf8") as defFile:
        for line in defFile.readlines():
            line = line.strip()
            if line == '; PROXY':
                is_proxy_section = True
            elif line.startswith(";"):
                is_proxy_section = False
            elif is_proxy_section:
                result.append(line)
    return result
print("Generating proxy template...")
parser = argparse.ArgumentParser(description='Generate proxy calls.')
parser.add_argument('-projectPath', required=True)
parser.add_argument('-templatePath', required=True)
parser.add_argument('-arch', required=True)
args = parser.parse_args()
projectPath = args.projectPath
templatePath = args.templatePath
arch = args.arch

ptrType = ""
if arch == "32":
    ptrType = "DWORD"
if arch == "64":
    ptrType = "QWORD"

defFilePath = os.path.join(projectPath, "dll.def")
defFile = parse_def_file(defFilePath)
sbDefs = ""
sbVariables = ""
sbJmps = ""

sbProxyDefs = ""
sbProxyAdd = ""

for func in defFile:
    func = func.strip()
    sbDefs += f"public {func}\n"
    sbDefs += f"public __{func}__\n"
    sbVariables += f"  __{func}__ {ptrType} 0\n"
    sbJmps += f"{func}:\n"
    sbJmps += f"  jmp {ptrType} ptr __{func}__\n"
    sbProxyDefs += f"extern FARPROC __{func}__;\n"
    sbProxyAdd += f'  __{func}__ = lookup["{func}"];\n'

tokens = {
    "definitions" : sbDefs,
    "variables" : sbVariables,
    "jmps" : sbJmps,
    "proxyDefs" : sbProxyDefs,
    "proxyAdd" : sbProxyAdd,
}
asmTemplatePath  = os.path.join(templatePath , "dllproxy.asm.tmpl")
asmTemplate  = open(asmTemplatePath, "r", encoding="utf8").read()
asm = merge_tokens(asmTemplate, tokens)
asmPath = os.path.join(projectPath , "dllproxy.asm")
with open(asmPath, "w", encoding="utf8") as file:
    file.write(asm)

cTemplatePath = os.path.join(templatePath, "proxy.cpp.tmpl")
cTemplate = open(cTemplatePath, "r", encoding="utf8").read()
cProxy = merge_tokens(cTemplate, tokens)
cPath = os.path.join(projectPath, "proxy.cpp")
with open(cPath, "w", encoding="utf8") as file:
    file.write(cProxy)
