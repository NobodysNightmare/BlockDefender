﻿using System;
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
            None = 0, Join = 1, Welcome = 2
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
                case PacketType.Join:
                    return new JoinPacket();
                case PacketType.Welcome:
                    return new WelcomePacket();
                default:
                    throw new UnsupportedPacketException();
            }
        }

        internal static void WritePacket(NetworkPacket packet, Stream stream)
        {
            var writer = new BinaryWriter(stream);
            SendPacketIdentifier(writer, packet);
            packet.WriteTo(writer);
        }

        private static void SendPacketIdentifier(BinaryWriter writer, NetworkPacket packet)
        {
            if (packet is JoinPacket)
                writer.Write((byte)PacketType.Join);
            else if (packet is WelcomePacket)
                writer.Write((byte)PacketType.Welcome);
        }
    }
}
