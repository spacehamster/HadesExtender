using System;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using Dia2Lib;
using System.Runtime.InteropServices;

namespace HadesExtender
{
    public class DbgHelpSymbolResolver : SymbolResolver
    {
        const int MaxSymbolName = 2000;

        readonly ProcessModule targetModule;

        readonly IntPtr resolverId;

        readonly ulong BaseOfDll;

        static object lockObj = new object();

        private List<string> m_EnumSymbolList;

        [DllImport("DbgHelp", SetLastError = true)]
        extern static uint SymSetOptions(SymOptions options);

        [DllImport("DbgHelp", SetLastError = true)]
        extern static bool SymInitialize(IntPtr hProcess, [MarshalAs(UnmanagedType.LPStr)] string userSearchPath, bool fInvadeProcess);

        [DllImport("DbgHelp", SetLastError = true)]
        extern static bool SymFromName(IntPtr hProcess, [MarshalAs(UnmanagedType.LPStr)] string name, ref SymbolInfo symbol);

        [DllImport("DbgHelp", SetLastError = true)]
        extern static ulong SymLoadModuleEx(IntPtr hProcess,
                IntPtr hFile,
                [MarshalAs(UnmanagedType.LPStr)] string imageName,
                [MarshalAs(UnmanagedType.LPStr)] string moduleName,
                ulong baseOfDll,
                uint dllSize,
                IntPtr data,
                uint flags);

        [DllImport("DbgHelp", SetLastError = true)]
        extern static bool SymEnumSymbols(IntPtr process, 
            ulong baseOfDll,
            [MarshalAs(UnmanagedType.LPStr)] string mask,
            IntPtr enumSymbolCallback,
            IntPtr userContext);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate bool EnumSymCallbackDelegate(ref SymbolInfo symInfo, ulong symbolSize, IntPtr userContext);

        readonly ConcurrentDictionary<string, IntPtr> cache;

        public DbgHelpSymbolResolver(ProcessModule module)
        {

            targetModule = module;
            resolverId = Process.GetCurrentProcess().MainModule.BaseAddress;
            if (targetModule.BaseAddress == resolverId)
                throw new InvalidOperationException($"DbgHelpSymbolResolver cannot be initialized from target module");
            cache = new ConcurrentDictionary<string, IntPtr>();

            lock (lockObj)
            {
                SymSetOptions(SymOptions.DeferedLoads | SymOptions.PublicsOnly);
                if (!SymInitialize(resolverId, null, false))
                {
                    DbgHelpError("Error calling SymInitialize");
                }
                BaseOfDll = SymLoadModuleEx(resolverId,
                    IntPtr.Zero,
                    targetModule.ModuleName,
                    null,
                    (ulong)targetModule.BaseAddress,
                    0,
                    IntPtr.Zero,
                    0);
                if (BaseOfDll == 0)
                {
                    DbgHelpError("Error calling SymLoadModuleEx");
                }
            }

            return;
        }

        private unsafe bool EnumSymCallback(ref SymbolInfo symInfo, ulong symbolSize, IntPtr userContext)
        {
            string name;
            fixed (byte* ptr = symInfo.Name)
            {
                name = Marshal.PtrToStringAnsi(new IntPtr(ptr));
            }
            m_EnumSymbolList.Add(name);
            return true;
        }

        public override IEnumerable<string> FindSymbolsMatching(Regex expression)
        {

            m_EnumSymbolList = new List<string>();
            var callback = new EnumSymCallbackDelegate(EnumSymCallback);
            lock (lockObj)
            {
                if (!SymEnumSymbols(resolverId,
                    BaseOfDll,
                    expression.ToString(),
                    Marshal.GetFunctionPointerForDelegate(callback),
                    IntPtr.Zero))
                {
                    DbgHelpError($"Error calling SymEnumSymbols {expression}");
                }
            }
            var result = m_EnumSymbolList;
            m_EnumSymbolList = null;
            return result;
        }

        protected override IntPtr GetAddressOrZero(string name)
        {
            if (cache.TryGetValue(name, out IntPtr address))
                return address;

            SymbolInfo symbol = new SymbolInfo();
            symbol.SizeOfStruct = (uint)(Marshal.SizeOf(typeof(SymbolInfo)) - MaxSymbolName);
            symbol.MaxNameLen = MaxSymbolName;

            lock (lockObj)
            {
                if (!SymFromName(resolverId, name, ref symbol))
                {
                    DbgHelpError($"Error calling SynFromName {name}");
                }
            }

            address = new IntPtr((long)symbol.Address);
            cache.TryAdd(name, address);
            return address;
        }

        private static void DbgHelpError(string message)
        {
            var err = Marshal.GetLastWin32Error();
            string errorMessage = new System.ComponentModel.Win32Exception(err).Message;
            throw new InvalidOperationException($"{message} (win32 error={err}. {errorMessage})");
        }

        private unsafe struct SymbolInfo
        {
            public uint SizeOfStruct;
            public uint TypeIndex;
            public fixed ulong Reserved[2];
            public uint Index;
            public uint Size;
            public ulong ModBase;
            public uint Flags;
            public ulong Value;
            public ulong Address;
            public uint Register;
            public uint Scope;
            public uint Tag;
            public uint NameLen;
            public uint MaxNameLen;
            public fixed byte Name[MaxSymbolName + 1];
        }

        private enum SymOptions : uint
        {
            CaseInsensitive         = 0x00000001,
            UndName                 = 0x00000002,
            DeferedLoads            = 0x00000004,
            NoCpp                   = 0x00000008,
            LoadLines               = 0x00000010,
            LoadAnything            = 0x00000040,
            IgnoreCvrec             = 0x00000080,
            NoUnqualifiedLoads      = 0x00000100,
            FailCriticalErrors      = 0x00000200,
            ExactSymbols            = 0x00000400,
            AllowAbsoluteSymbols    = 0x00000800,
            IgnoreNtSympath         = 0x00001000,
            Include32bitModules     = 0x00002000,
            PublicsOnly             = 0x00004000,
            NoPublics               = 0x00008000,
            AutoPublics             = 0x00010000,
            NoImageSearch           = 0x00020000,
            Secure                  = 0x00040000,
            NoPrompts               = 0x00080000,
            NoOverwrite             = 0x00100000,
            IgnoreImagedir          = 0x00200000,
            FlatDirectory           = 0x00400000,
            FavorCompressed         = 0x00800000,
            AllowZeroAddress        = 0x01000000,
            DisableSymsrvAutodetect = 0x02000000,
            Debug                   = 0x80000000,
        }
    }
}
