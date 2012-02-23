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
            None = 0, JoinRequest = 1, Welcome = 2
        }
        internal static NetworkPacket ReadPacket(Stream stream)
        {
            var reader = new BinaryReader(stream); //do not dispose, stream has to stay open
            NetworkPacket packet = InstanciatePacket(reader);
            packet.ReadFrom(reader);
            return packet;
        }

        private static NetworkPacket InstanciatePacket(BinaryReader reader)
        {
            PacketType type = (PacketType)reader.ReadByte();
            switch (type)
            {
                case PacketType.JoinRequest:
                    return new JoinRequestPacket();
                case PacketType.Welcome:
                    return new WelcomePacket();
                default:
                    throw new UnsupportedPacketException();
            }
        }

        internal static void WritePacket(NetworkPacket packet, Stream stream)
        {
            var writer = new BinaryWriter(stream);  //do not dispose, stream has to stay open
            SendPacketIdentifier(writer, packet);
            packet.WriteTo(writer);
        }

        private static void SendPacketIdentifier(BinaryWriter writer, NetworkPacket packet)
        {
            if (packet is JoinRequestPacket)
                writer.Write((byte)PacketType.JoinRequest);
            else if (packet is WelcomePacket)
                writer.Write((byte)PacketType.Welcome);
        }
    }
}
