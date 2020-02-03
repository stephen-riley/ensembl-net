using System;

namespace Ensembl.Exceptions
{
    public class EnsemblException : Exception
    {
        public EnsemblException(string message) : base(message)
        {
        }
    }
}