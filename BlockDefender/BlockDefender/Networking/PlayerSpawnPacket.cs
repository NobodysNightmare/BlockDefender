using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;

namespace BlockDefender.Networking
{
    class PlayerSpawnPacket : NetworkPacket
    {
        public Vector2 Position { get; private set; }

        public PlayerSpawnPacket()
        { }

        public PlayerSpawnPacket(Vector2 position)
        {
            Position = position;
        }

        internal override void ReadFrom(BinaryReader reader)
        {
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            Position = new Vector2(x, y);
        }

        internal override void WriteTo(BinaryWriter writer)
        {
            writer.Write(Position.X);
            writer.Write(Position.Y);
        }
    }
}
