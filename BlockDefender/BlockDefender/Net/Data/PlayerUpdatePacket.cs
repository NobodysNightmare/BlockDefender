using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;

namespace BlockDefender.Net.Data
{
    class PlayerUpdatePacket : NetworkPacket
    {
        public int PlayerId { get; private set; }
        public Vector2 Position { get; private set; }
        public PlayerHeading Heading { get; private set; }

        internal override void ReadFrom(BinaryReader reader)
        {
            PlayerId = reader.ReadInt32();
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            Position = new Vector2(x, y);
            Heading = (PlayerHeading)reader.ReadByte();
        }

        internal override void WriteTo(BinaryWriter writer)
        {
            writer.Write(PlayerId);
            writer.Write(Position.X);
            writer.Write(Position.Y);
            writer.Write((byte)Heading);
        }
    }
}
