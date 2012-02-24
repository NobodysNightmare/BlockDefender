using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlockDefender
{
    class UnsupportedDataException : Exception
    {
        public UnsupportedDataException(string message)
            : base(message)
        { }

        public UnsupportedDataException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
