using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace HadesExtender
{
    public unsafe class ExportResolver : SymbolResolver
    {
        const int IMAGE_DOS_SIGNATURE = 0x5A4D; // MZZ
        const int IMAGE_NT_SIGNATURE = 0x00004550; // PE00
        IMAGE_DOS_HEADER* lib;
        IMAGE_NT_HEADERS* header;
        IMAGE_EXPORT_DIRECTORY* exports;
        public ExportResolver(ProcessModule module)
        {
            lib = (IMAGE_DOS_HEADER*)module.BaseAddress;
            if (lib->e_magic != IMAGE_DOS_SIGNATURE) throw new Exception("Invalid IMAGE_DOS_HEADER signature");
            header = (IMAGE_NT_HEADERS*)((byte*)lib + lib->e_lfanew);
            if (header->Signature != IMAGE_NT_SIGNATURE) throw new Exception("Invalid IMAGE_NT_HEADERS signature");
            if (header->OptionalHeader.NumberOfRvaAndSizes == 0) throw new Exception("Invalid NumberOfRvaAndSizes");
            exports = (IMAGE_EXPORT_DIRECTORY*)((byte*)lib + header->OptionalHeader.ExportTable.VirtualAddress);
        }

        protected override IntPtr GetAddressOrZero(string target)
        {
            if (exports->AddressOfNames == 0) throw new Exception("Invalid AddressOfNames");
            if (exports->AddressOfFunctions == 0) throw new Exception("Invalid AddressOfFunctions");
            uint* names = (uint*)((byte*)lib + exports->AddressOfNames);
            uint* functions = (uint*)((byte*)lib + exports->AddressOfFunctions);
            for (int i = 0; i < exports->NumberOfNames; i++)
            {
                var name = Marshal.PtrToStringAnsi(new IntPtr((byte*)lib + names[i]));
                if (name == target)
                {
                    return new IntPtr((byte*)lib + functions[i]);
                }
            }
            return IntPtr.Zero;
        }

        public override IEnumerable<string> FindSymbolsMatching(Regex expression)
        {
            if (exports->AddressOfNames == 0) throw new Exception("Invalid AddressOfNames");
            uint* names = (uint*)((byte*)lib + exports->AddressOfNames);
            List<string> result = new List<string>();
            for (int i = 0; i < exports->NumberOfNames; i++)
            {
                var name = Marshal.PtrToStringAnsi(new IntPtr((byte*)lib + names[i]));
                if (expression.IsMatch(name))
                {
                    result.Add(name);
                }
            }
            return result;
        }

        public unsafe struct IMAGE_DOS_HEADER
        {
            public ushort e_magic;       // Magic number
            public ushort e_cblp;    // Bytes on last page of file
            public ushort e_cp;      // Pages in file
            public ushort e_crlc;    // Relocations
            public ushort e_cparhdr;     // Size of header in paragraphs
            public ushort e_minalloc;    // Minimum extra paragraphs needed
            public ushort e_maxalloc;    // Maximum extra paragraphs needed
            public ushort e_ss;      // Initial (relative) SS value
            public ushort e_sp;      // Initial SP value
            public ushort e_csum;    // Checksum
            public ushort e_ip;      // Initial IP value
            public ushort e_cs;      // Initial (relative) CS value
            public ushort e_lfarlc;      // File address of relocation table
            public ushort e_ovno;    // Overlay number
            public fixed ushort e_res1[4];    // Reserved words
            public ushort e_oemid;       // OEM identifier (for e_oeminfo)
            public ushort e_oeminfo;     // OEM information; e_oemid specific
            public fixed ushort e_res2[10];    // Reserved words
            public ushort e_lfanew;      // File address of new exe header
        }

        public unsafe struct IMAGE_NT_HEADERS
        {
            public uint Signature;
            public IMAGE_FILE_HEADER FileHeader;
            public IMAGE_OPTIONAL_HEADER OptionalHeader;
        }

        public enum MachineType : ushort
        {
            Native = 0,
            I386 = 0x014c,
            Itanium = 0x0200,
            x64 = 0x8664
        }
        public enum MagicType : ushort
        {
            IMAGE_NT_OPTIONAL_HDR32_MAGIC = 0x10b,
            IMAGE_NT_OPTIONAL_HDR64_MAGIC = 0x20b
        }
        public enum SubSystemType : ushort
        {
            IMAGE_SUBSYSTEM_UNKNOWN = 0,
            IMAGE_SUBSYSTEM_NATIVE = 1,
            IMAGE_SUBSYSTEM_WINDOWS_GUI = 2,
            IMAGE_SUBSYSTEM_WINDOWS_CUI = 3,
            IMAGE_SUBSYSTEM_POSIX_CUI = 7,
            IMAGE_SUBSYSTEM_WINDOWS_CE_GUI = 9,
            IMAGE_SUBSYSTEM_EFI_APPLICATION = 10,
            IMAGE_SUBSYSTEM_EFI_BOOT_SERVICE_DRIVER = 11,
            IMAGE_SUBSYSTEM_EFI_RUNTIME_DRIVER = 12,
            IMAGE_SUBSYSTEM_EFI_ROM = 13,
            IMAGE_SUBSYSTEM_XBOX = 14
        }
        public enum DllCharacteristicsType : ushort
        {
            RES_0 = 0x0001,
            RES_1 = 0x0002,
            RES_2 = 0x0004,
            RES_3 = 0x0008,
            IMAGE_DLL_CHARACTERISTICS_DYNAMIC_BASE = 0x0040,
            IMAGE_DLL_CHARACTERISTICS_FORCE_INTEGRITY = 0x0080,
            IMAGE_DLL_CHARACTERISTICS_NX_COMPAT = 0x0100,
            IMAGE_DLLCHARACTERISTICS_NO_ISOLATION = 0x0200,
            IMAGE_DLLCHARACTERISTICS_NO_SEH = 0x0400,
            IMAGE_DLLCHARACTERISTICS_NO_BIND = 0x0800,
            RES_4 = 0x1000,
            IMAGE_DLLCHARACTERISTICS_WDM_DRIVER = 0x2000,
            IMAGE_DLLCHARACTERISTICS_TERMINAL_SERVER_AWARE = 0x8000
        }

        public struct IMAGE_OPTIONAL_HEADER
        {
            public MagicType Magic;
            public byte MajorLinkerVersion;
            public byte MinorLinkerVersion;
            public uint SizeOfCode;
            public uint SizeOfInitializedData;
            public uint SizeOfUninitializedData;
            public uint AddressOfEntryPoint;
            public uint BaseOfCode;
            public ulong ImageBase;
            public uint SectionAlignment;
            public uint FileAlignment;
            public ushort MajorOperatingSystemVersion;
            public ushort MinorOperatingSystemVersion;
            public ushort MajorImageVersion;
            public ushort MinorImageVersion;
            public ushort MajorSubsystemVersion;
            public ushort MinorSubsystemVersion;
            public uint Win32VersionValue;
            public uint SizeOfImage;
            public uint SizeOfHeaders;
            public uint CheckSum;
            public SubSystemType Subsystem;
            public DllCharacteristicsType DllCharacteristics;
            public IntPtr SizeOfStackReserve;
            public IntPtr SizeOfStackCommit;
            public IntPtr SizeOfHeapReserve;
            public IntPtr SizeOfHeapCommit;
            public uint LoaderFlags;
            public uint NumberOfRvaAndSizes;
            public IMAGE_DATA_DIRECTORY ExportTable;
            public IMAGE_DATA_DIRECTORY ImportTable;
            public IMAGE_DATA_DIRECTORY ResourceTable;
            public IMAGE_DATA_DIRECTORY ExceptionTable;
            public IMAGE_DATA_DIRECTORY CertificateTable;
            public IMAGE_DATA_DIRECTORY BaseRelocationTable;
            public IMAGE_DATA_DIRECTORY Debug;
            public IMAGE_DATA_DIRECTORY Architecture;
            public IMAGE_DATA_DIRECTORY GlobalPtr;
            public IMAGE_DATA_DIRECTORY LoadConfigTable;
            public IMAGE_DATA_DIRECTORY BoundImport;
            public IMAGE_DATA_DIRECTORY IAT;
            public IMAGE_DATA_DIRECTORY DelayImportDescriptor;
            public IMAGE_DATA_DIRECTORY CLRRuntimeHeader;
            public IMAGE_DATA_DIRECTORY Reserved;
        }

        public struct IMAGE_DATA_DIRECTORY
        {
            public uint VirtualAddress;
            public uint Size;
        }

        public struct IMAGE_FILE_HEADER
        {
            public ushort Machine;
            public ushort NumberOfSections;
            public uint TimeDateStamp;
            public uint PointerToSymbolTable;
            public uint NumberOfSymbols;
            public ushort SizeOfOptionalHeader;
            public ushort Characteristics;
        }

        public struct IMAGE_EXPORT_DIRECTORY
        {
            public uint Characteristics;
            public uint TimeDateStamp;
            public ushort MajorVersion;
            public ushort MinorVersion;
            public uint Name;
            public uint Base;
            public uint NumberOfFunctions;
            public uint NumberOfNames;
            public uint AddressOfFunctions;     // RVA from base of image
            public uint AddressOfNames;     // RVA from base of image
            public uint AddressOfNameOrdinals;  // RVA from base of image
        }
    }
}
