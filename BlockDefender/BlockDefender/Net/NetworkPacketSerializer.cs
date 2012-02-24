using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using BlockDefender.Net.Data;
using System.Net.Sockets;

namespace BlockDefender.Net
{
    class NetworkPacketSerializer
    {
        private enum PacketType : byte
        {
            None, JoinRequest, Welcome, PlayerSpawn, AssignPlayer, PlayerUpdate
        }

        internal static NetworkPacket ReadPacket(Socket socket)
        {
            using (var stream = new NetworkStream(socket))
            using (var reader = new BinaryReader(stream))
            {
                NetworkPacket packet = InstanciatePacket(reader);
                packet.ReadFrom(reader);
                return packet;
            }
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
                case PacketType.PlayerSpawn:
                    return new PlayerSpawnPacket();
                case PacketType.AssignPlayer:
                    return new AssignPlayerPacket();
                case PacketType.PlayerUpdate:
                    return new PlayerUpdatePacket();
                default:
                    throw new UnsupportedPacketException();
            }
        }

        internal static void WritePacket(NetworkPacket packet, Socket socket)
        {
            using (var stream = new NetworkStream(socket))
            using (var writer = new BinaryWriter(stream))
            {
                SendPacketIdentifier(writer, packet);
                packet.WriteTo(writer);
            }
        }

        private static void SendPacketIdentifier(BinaryWriter writer, NetworkPacket packet)
        {
            if (packet is JoinRequestPacket)
                writer.Write((byte)PacketType.JoinRequest);
            else if (packet is WelcomePacket)
                writer.Write((byte)PacketType.Welcome);
            else if (packet is PlayerSpawnPacket)
                writer.Write((byte)PacketType.PlayerSpawn);
            else if (packet is AssignPlayerPacket)
                writer.Write((byte)PacketType.AssignPlayer);
            else if (packet is PlayerUpdatePacket)
                writer.Write((byte)PacketType.PlayerUpdate);
        }
    }
}
