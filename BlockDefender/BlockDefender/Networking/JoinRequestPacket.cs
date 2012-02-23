using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BlockDefender.Networking
{
    class JoinRequestPacket : NetworkPacket
    {
        internal override void ReadFrom(BinaryReader reader)
        { }

        internal override void WriteTo(BinaryWriter writer)
        { }
    }
}
