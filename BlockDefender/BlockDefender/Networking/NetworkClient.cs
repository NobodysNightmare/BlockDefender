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
        private Player AssignedPlayer;

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

        public void MovePlayer(PlayerHeading direction)
        {
            if (AssignedPlayer == null)
                return;

            AssignedPlayer.Move(direction);
            //TODO: send position-update to server
            //TODO: move/interact might be pulled into own interface implemented by Player and some kind of network-decorator for Player
        }

        public void PlayerInteract()
        {
            //TODO: might need rework to work properly over network
            if (AssignedPlayer != null)
                AssignedPlayer.Interact();
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
            else if(packet is AssignPlayerPacket)
                ProcessPacket((AssignPlayerPacket)packet);
        }

        private void ProcessPacket(PlayerSpawnPacket spawnPacket)
        {
            var p = new Player(spawnPacket.Id, Playground, spawnPacket.Position);
            Playground.AddPlayer(p);
        }

        private void ProcessPacket(AssignPlayerPacket assignPacket)
        {
            AssignedPlayer = Playground.Players.Single(player => player.Id == assignPacket.PlayerId);
        }

        public void Dispose()
        {
            Stream.Dispose();
        }
    }
}
