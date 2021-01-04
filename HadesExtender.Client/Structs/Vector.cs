using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HadesExtender.Structs
{
    public unsafe struct Vector
    {
        fixed byte Data[24];
    }
}
