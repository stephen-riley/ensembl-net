using System.Reflection;

namespace Ensembl
{
    public class NAssembly : Dto.Assembly
    {
        public NAssembly(int asmStart, int asmEnd, int cmpStart, int cmpEnd)
        : base(-1, asmStart, asmEnd, 0, cmpStart, cmpEnd, 1)
        {
        }
    }
}