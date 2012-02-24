using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using BlockDefender.Net.Data;

namespace BlockDefender.Net
{
    class NetworkClient : IDisposable
    {
        private Socket Socket;
        private Playground Playground;
        private Player AssignedPlayer;

        public NetworkClient()
        {
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            Socket.Connect(AppSettings.Default.ConnectHost, AppSettings.Default.ConnectPort);
        }

        public Playground EstablishConnection()
        {
            NetworkPacketSerializer.WritePacket(new JoinRequestPacket(), Socket);
            var packet = NetworkPacketSerializer.ReadPacket(Socket);
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
            NetworkPacketSerializer.WritePacket(new PlayerUpdatePacket(AssignedPlayer), Socket);
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
            while (Socket.Available > 0)
            {
                NetworkPacket packet = NetworkPacketSerializer.ReadPacket(Socket);
                ReceiveUpdate(packet);
            }
        }

        private void ReceiveUpdate(NetworkPacket packet)
        {
            if (packet is PlayerSpawnPacket)
                ProcessPacket((PlayerSpawnPacket)packet);
            else if(packet is AssignPlayerPacket)
                ProcessPacket((AssignPlayerPacket)packet);
            else if (packet is PlayerUpdatePacket)
                ProcessPacket((PlayerUpdatePacket)packet);
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

        private void ProcessPacket(PlayerUpdatePacket updatePacket)
        {
            Playground.Players.Single(player => player.Id == updatePacket.Id).Update(updatePacket.Position, updatePacket.Heading);
        }

        public void Dispose()
        {
            Socket.Dispose();
        }
    }
}
