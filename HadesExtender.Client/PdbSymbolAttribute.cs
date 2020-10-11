using System;

namespace HadesExtender
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal sealed class PdbSymbolAttribute : Attribute
    {
        public string SymbolName { get; }

        public PdbSymbolAttribute(string symbolName)
        {
            SymbolName = symbolName;
        }

        public PdbSymbolAttribute()
        {
        }
    }
}
