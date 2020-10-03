using System;
using System.IO;

namespace HadesExtender
{
    public class IpcInterface : MarshalByRefObject
    {
        public TextReader In { get; }

        public TextWriter Out { get; }

        public TextWriter Error { get; }

        public IpcInterface(TextReader @in, TextWriter @out, TextWriter error)
        {
            In              = @in;
            Out             = @out;
            Error           = error;
        }

        public void Ping()
        {
        }
    }
}
