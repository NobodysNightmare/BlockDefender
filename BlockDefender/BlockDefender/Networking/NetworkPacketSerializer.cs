using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BlockDefender.Networking
{
    class NetworkPacketSerializer
    {
        private enum PacketType : byte
        {
            None, Join, Welcome
        }
        internal static NetworkPacket ReadPacket(Stream stream)
        {
            NetworkPacket packet = null;
            using (var reader = new BinaryReader(stream))
            {
                packet = InstanciatePacket(reader);
                packet.ReadFrom(reader);
            }
            return packet;
        }

        private static NetworkPacket InstanciatePacket(BinaryReader reader)
        {
            PacketType type = (PacketType)reader.ReadByte();
            switch (type)
            {
                case PacketType.Join:
                    return new JoinPacket();
                default:
                    throw new UnsupportedPacketException();
            }
        }

        internal static void WritePacket(NetworkPacket packet, Stream stream)
        {
            using (var writer = new BinaryWriter(stream))
            {
                SendPacketIdentifier(writer, packet);
                packet.WriteTo(writer);
            }
        }

        private static void SendPacketIdentifier(BinaryWriter writer, NetworkPacket packet)
        {
            if (packet is JoinPacket)
                writer.Write((byte)PacketType.Join);
        }
    }
}
