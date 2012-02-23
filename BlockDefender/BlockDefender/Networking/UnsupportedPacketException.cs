using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlockDefender.Networking
{
    class UnsupportedPacketException : Exception
    {
        public UnsupportedPacketException()
            : base("This kind of network packet is not supported by this server.")
        { }
    }
}
