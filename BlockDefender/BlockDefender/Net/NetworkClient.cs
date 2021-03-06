﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using BlockDefender.Net.Data;
using BlockDefender.Terrain;
using System.Net;

namespace BlockDefender.Net
{
    delegate void MapChangedEventHandler(object source, MapChangedEventArgs e);
    delegate void PlayerAssignedEventHandler(object source, PlayerAssignedEventArgs e);
    class MapChangedEventArgs : EventArgs
    {
        public Map Map { get; private set; }

        public MapChangedEventArgs(Map map)
        {
            Map = map;
        }
    }

    class PlayerAssignedEventArgs : EventArgs
    {
        public Player Player { get; private set; }

        public PlayerAssignedEventArgs(Player player)
        {
            Player = player;
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
        public event PlayerAssignedEventHandler PlayerAssigned;

        public NetworkClient()
        {
            State = NetworkClientState.Disconnected;
            Socket = new Socket(AddressFamily.InterNetwork | AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.IP);
        }

        public void EstablishConnection(string host, int port)
        {
            State = NetworkClientState.Initializing;
            SocketAsyncEventArgs connectArgs = new SocketAsyncEventArgs();
            connectArgs.RemoteEndPoint = new IPEndPoint(Dns.GetHostEntry(host).AddressList.First(), port);
            connectArgs.Completed += OnConnectionEstablished;
            if(!Socket.ConnectAsync(connectArgs))
                OnConnectionEstablished(this, connectArgs);
        }

        void OnConnectionEstablished(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success)
            {
                State = NetworkClientState.Disconnected;
                return;
            }
            NetworkPacketSerializer.WritePacket(new JoinRequestPacket(), Socket);
            var packet = NetworkPacketSerializer.ReadPacket(Socket);
            var welcome = packet as WelcomePacket;
            if (packet == null)
            {
                throw new Exception("Expected a welcome packet but received something else!");
            }
            Playground = new Playground(welcome.Map);
            RaiseMapChanged(welcome.Map);
            State = NetworkClientState.Ready;
        }

        private void RaiseMapChanged(Map map)
        {
            if (MapChanged != null)
            {
                MapChanged(this, new MapChangedEventArgs(map));
            }
        }

        private void RaisePlayerAssigned(Player player)
        {
            if (PlayerAssigned != null)
            {
                PlayerAssigned(this, new PlayerAssignedEventArgs(player));
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
            RaisePlayerAssigned(AssignedPlayer);
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
