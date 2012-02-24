using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlockDefender.Net
{
    class UnsupportedPacketException : UnsupportedDataException
    {
        public UnsupportedPacketException()
            : base("The received packet could not be parsed")
        { }
    }
}
