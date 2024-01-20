using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sldc.Exceptions
{
    public class UnmatchedDRPTokenException : Exception
    {
        public UnmatchedDRPTokenException() : base() { }

        public UnmatchedDRPTokenException(string message) : base(message) { }

        public UnmatchedDRPTokenException(string message, Exception innerException) : base(message, innerException) { }
    }
}
