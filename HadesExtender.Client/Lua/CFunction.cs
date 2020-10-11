using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HadesExtender
{
    public struct CFunction
    {
        IntPtr FuncPtr;

        public CFunction(IntPtr funcPtr)
        {
            FuncPtr = funcPtr;
        }

        public static implicit operator CFunction(IntPtr ptr)
        {
            return new CFunction(ptr);
        }

        public static implicit operator IntPtr(CFunction func)
        {
            return func.FuncPtr;
        }
    }
}
