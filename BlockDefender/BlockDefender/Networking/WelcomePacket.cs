using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BlockDefender.Networking
{
    class WelcomePacket : NetworkPacket
    {
        public Map Map { get; private set; }

        internal override void ReadFrom(BinaryReader reader)
        {
            Map = new MapReader(reader).ReadMap();
        }

        internal override void WriteTo(BinaryWriter writer)
        {
            new MapWriter(writer).WriteMap(Map);
        }
    }
}
