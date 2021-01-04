using System.Runtime.InteropServices;

namespace HadesExtender.Structs
{
    public unsafe struct BasicString
    {
        private fixed byte Data[24];

        private delegate void BasicString_Ctor_Delegate(ref BasicString _this, 
            [MarshalAs(UnmanagedType.LPStr)] string param);

        [PdbSymbol("??4?$basic_string@DVallocator_forge@eastl@@@eastl@@QEAAAEAV01@PEBD@Z")]
        private static BasicString_Ctor_Delegate BasicString_Ctor;

        public static BasicString Create(string str)
        {
            var basicString = new BasicString();
            BasicString_Ctor(ref basicString, str);
            return basicString;
        }
    }
}
