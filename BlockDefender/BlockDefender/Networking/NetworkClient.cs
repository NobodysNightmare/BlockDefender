using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace BlockDefender.Networking
{
    class NetworkClient : IDisposable
    {
        private NetworkStream Stream;
        private Playground Playground;

        public NetworkClient()
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            socket.Connect(AppSettings.Default.ConnectHost, AppSettings.Default.ConnectPort);
            Stream = new NetworkStream(socket, true);
        }

        public Playground EstablishConnection()
        {
            NetworkPacketSerializer.WritePacket(new JoinRequestPacket(), Stream);
            var packet = NetworkPacketSerializer.ReadPacket(Stream);
            var welcome = packet as WelcomePacket;
            if (packet != null)
            {
                Playground = new Playground(welcome.Map);
                return Playground;
            }
            throw new Exception("Expected a welcome packet but received something else!");
        }

        public void Update()
        {
            while (Stream.DataAvailable)
            {
                NetworkPacket packet = NetworkPacketSerializer.ReadPacket(Stream);
                ReceiveUpdate(packet);
            }
        }

        private void ReceiveUpdate(NetworkPacket packet)
        {
            if (packet is PlayerSpawnPacket)
                ProcessPacket((PlayerSpawnPacket)packet);
        }

        private void ProcessPacket(PlayerSpawnPacket spawnPacket)
        {
            var p = new Player(spawnPacket.Id, Playground, spawnPacket.Position);
            Playground.AddPlayer(p);
        }

        public void Dispose()
        {
            Stream.Dispose();
        }
    }
}
