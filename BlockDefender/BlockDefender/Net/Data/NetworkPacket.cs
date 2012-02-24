using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BlockDefender.Net.Data
{
    abstract class NetworkPacket
    {
        abstract internal void ReadFrom(BinaryReader reader);
        abstract internal void WriteTo(BinaryWriter writer);
    }
}
