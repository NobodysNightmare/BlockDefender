using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using BlockDefender.Net.Data;
using BlockDefender.Terrain;

namespace BlockDefender.Net
{
    delegate void MapChangedEventHandler(object source, MapChangedEventArgs e);
    class MapChangedEventArgs : EventArgs
    {
        public Map Map { get; private set; }

        public MapChangedEventArgs(Map map)
        {
            Map = map;
        }
    }

    enum NetworkClientState
    {
        Disconnected, Initializing, Ready
    }

    class NetworkClient : IDisposable
    {
        private Socket Socket;
        private Playground Playground;
        private Player AssignedPlayer;

        public IDrawableComponent Visual
        {
            get
            {
                return Playground;
            }
        }

        public NetworkClientState State { get; private set; }

        public event MapChangedEventHandler MapChanged;

        public NetworkClient()
        {
            State = NetworkClientState.Disconnected;
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
        }

        public void EstablishConnection(string host, int port)
        {
            Socket.Connect(host, port);
            NetworkPacketSerializer.WritePacket(new JoinRequestPacket(), Socket);
            var packet = NetworkPacketSerializer.ReadPacket(Socket);
            var welcome = packet as WelcomePacket;
            if (packet == null)
            {
                throw new Exception("Expected a welcome packet but received something else!");
            }
            Playground = new Playground(welcome.Map);
            OnMapChanged(welcome.Map);
            State = NetworkClientState.Ready;
        }

        private void OnMapChanged(Map map)
        {
            if (MapChanged != null)
            {
                MapChanged(this, new MapChangedEventArgs(map));
            }
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
            if (State != NetworkClientState.Ready)
                return;

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
