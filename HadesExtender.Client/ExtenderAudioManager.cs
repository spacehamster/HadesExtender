using HadesExtender.Structs;
using System;
using System.Runtime.InteropServices;

namespace HadesExtender
{
    public unsafe class ExtenderAudioManager
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void LoadBankDelegate(AudioManager* audioManager, [MarshalAs(UnmanagedType.U1)] bool param_1, 
            ref BasicString param_2, ref Vector param_3, [MarshalAs(UnmanagedType.U1)]  bool param_4);

        [PdbSymbol("?LoadBank@AudioManager@sgg@@AEAAX_NAEBV?$basic_string@DVallocator_forge@eastl@@@eastl@@AEAV?$vector@UBankInfo@AudioManager@sgg@@Vallocator_forge@eastl@@@4@0@Z")]
        static LoadBankDelegate LoadBankFunc;

        [PdbSymbol("?AUDIO_MANAGER@AudioManager@sgg@@0PEAV12@EA")]
        static IntPtr _AudioManagerPtr;

        static AudioManager* AudioManagerPtr => (AudioManager*)_AudioManagerPtr;

        public static void LoadBank(string path, bool value)
        {
            var basicStr = BasicString.Create(path);
            LoadBankFunc(AudioManagerPtr, value, ref basicStr, ref AudioManagerPtr->mGlobalBanks, false);
        }
    }
}
