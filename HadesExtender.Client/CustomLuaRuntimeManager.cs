using EasyHook;
using Reloaded.Hooks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HadesExtender
{
    public class CustomLuaRuntimeManager
    {
        List<string> unresolvedSymbols = new List<string>();
        Dictionary<string, object> luahooks = new Dictionary<string, object>();
        public void Init(SymbolResolver resolver)
        {
            Kernel32.LoadLibrary(Path.Combine(Util.ExtenderDirectory, "Lua.dll"));
            var luaModule = Util.GetModule("Lua.dll");
            var luaResolver = new DiaSymbolResolver(luaModule);
            using var sw = new StreamWriter("PatchLog.txt");
            var useEasyhook = false;
            //Debugger.Launch();
            var symbols = resolver.FindSymbolsMatching(new Regex("lua*"))
                .Concat(resolver.FindSymbolsMatching(new Regex(@"\?lua*")));
            foreach (var symbol in symbols)
            {
                var source = resolver.Resolve(symbol);
                if (!luaResolver.TryResolve(symbol, out var target))
                {
                    RegisterErrorHook(symbol, source);
                    sw.WriteLine($"Could not find symbol {symbol} in lua.dll");
                    continue;
                }
                if (useEasyhook)
                {
                    try
                    {
                        var hook = LocalHook.CreateUnmanaged(source, target, IntPtr.Zero);
                        hook.ThreadACL.SetExclusiveACL(Array.Empty<int>());
                        luahooks[symbol] = hook;
                        sw.WriteLine($"hooked lua function {symbol}");
                    }
                    catch (NotSupportedException ex)
                    {
                        sw.WriteLine($"Could not hook {symbol} - {ex.Message}");
                    }
                }
                else
                {
                    /*TODO: exception from Utilities.GetAbsoluteJumpMnemonics(target, is64bit:true)
                    System.NullReferenceException: Object reference not set to an instance of an object.
                       at System.SpanHelpers.CopyTo[T](T & dst, Int32 dstLength, T & src, Int32 srcLength)
                       at System.Span`1.TryCopyTo(Span`1 destination)
                       at Reloaded.Hooks.Internal.Patch.ApplyUnsafe()
                       at Reloaded.Hooks.AsmHook.Activate() */
                    var asm = new string[] {
                        $"use64",
                        $"mov rax, {target.ToInt64()}",
                        $"jmp rax",
                    };
                    var hook = new AsmHook(asm, source.ToInt64(), Reloaded.Hooks.Definitions.Enums.AsmHookBehaviour.DoNotExecuteOriginal).Activate();
                    luahooks[symbol] = hook;
                    sw.WriteLine($"hooked lua function {symbol}");
                }
            }
        }

        [Reloaded.Hooks.Definitions.X64.Function(Reloaded.Hooks.Definitions.X64.CallingConventions.Microsoft)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void OnErrorDelegate(int index);

        public void OnError(int index)
        {
            var name = unresolvedSymbols[index];
            Console.WriteLine($"Unresolved lua symbol called: {name}");
        }
        Reloaded.Hooks.X64.ReverseWrapper<OnErrorDelegate> OnErrorWrapper;

        public void RegisterErrorHook(string symbol, IntPtr source)
        {
            if(OnErrorWrapper == null)
            {
                OnErrorWrapper = new Reloaded.Hooks.X64.ReverseWrapper<OnErrorDelegate>(OnError);
            }

            var asm = new string[] {
                        $"use64",
                        $"mov ecx, {unresolvedSymbols.Count}",
                        $"mov rax, {OnErrorWrapper.WrapperPointer}",
                        $"jmp rax",
                    };
            var hook = new AsmHook(asm, source.ToInt64(), Reloaded.Hooks.Definitions.Enums.AsmHookBehaviour.DoNotExecuteOriginal).Activate();
            luahooks[symbol] = hook;
            unresolvedSymbols.Add(symbol);
        }
    }
}
