using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlockDefender.Net
{
    class UnsupportedPacketException : Exception
    {
        public UnsupportedPacketException()
            : base("This kind of network packet is not supported by this server.")
        { }
    }
}
