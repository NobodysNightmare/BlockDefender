using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BlockDefender.Net.Data
{
    class AssignPlayerPacket : NetworkPacket
    {
        public int PlayerId { get; private set; }

        public AssignPlayerPacket()
        { }

        public AssignPlayerPacket(int playerId)
        {
            PlayerId = playerId;
        }

        internal override void ReadFrom(BinaryReader reader)
        {
            PlayerId = reader.ReadInt32();
        }

        internal override void WriteTo(BinaryWriter writer)
        {
            writer.Write(PlayerId);
        }
    }
}
