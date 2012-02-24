using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;

namespace BlockDefender.Net.Data
{
    class PlayerSpawnPacket : NetworkPacket
    {
        public int Id { get; private set; }
        public Vector2 Position { get; private set; }

        public PlayerSpawnPacket()
        { }

        public PlayerSpawnPacket(int id, Vector2 position)
        {
            Id = id;
            Position = position;
        }

        internal override void ReadFrom(BinaryReader reader)
        {
            Id = reader.ReadInt32();
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            Position = new Vector2(x, y);
        }

        internal override void WriteTo(BinaryWriter writer)
        {
            writer.Write(Id);
            writer.Write(Position.X);
            writer.Write(Position.Y);
        }
    }
}
