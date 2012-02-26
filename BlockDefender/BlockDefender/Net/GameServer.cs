using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.IO;
using BlockDefender.Net.Data;
using Microsoft.Xna.Framework;
using BlockDefender.Terrain;

namespace BlockDefender.Net
{
    class GameServer
    {
        private const int BacklogSize = 5;

        private Thread ServerThread;
        private Map Map;
        private Playground Playground;
        private Socket ListenSocket;
        private List<Socket> ActiveSockets;

        public GameServer(Map map, int port)
        {
            Map = map;
            Playground = new Playground(Map);

            ActiveSockets = new List<Socket>();
            //FIXME: how to enable ipv4 (dual-stack) support?
            ListenSocket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.IP);
            ListenSocket.Bind(new IPEndPoint(IPAddress.IPv6Any, port));
            ActiveSockets.Add(ListenSocket);
        }

        public void Start()
        {
            ServerThread = new Thread(RunServerLoop);
            ServerThread.Name = "GameServer";
            ServerThread.IsBackground = true;
            ServerThread.Start();
        }

        public void Stop()
        {
            if(ServerThread != null && ServerThread.IsAlive)
                ServerThread.Abort();
        }

        private void RunServerLoop()
        {
            ListenSocket.Listen(BacklogSize);
            try
            {
                while (true)
                {
                    RunSingleLoop();
                }
            }
            catch (ThreadAbortException)
            {
                //TODO: clean up & close all connections
            }
        }

        private void RunSingleLoop()
        {
            var readableSockets = new List<Socket>(ActiveSockets);
            Socket.Select(readableSockets, null, null, Timeout.Infinite);
            foreach (var socket in readableSockets)
            {
                if (socket == ListenSocket)
                    AcceptNewConnection();
                else
                    ReceivePacket(socket);
            }
        }

        private void AcceptNewConnection()
        {
            ActiveSockets.Add(ListenSocket.Accept());
        }

        private void ReceivePacket(Socket socket)
        {
            try
            {
                NetworkPacket packet = NetworkPacketSerializer.ReadPacket(socket);
                ProcessPacket(packet, socket);
            }
            catch (IOException)
            {
                DropConnection(socket);
            }
            catch (UnsupportedPacketException)
            {
                DropConnection(socket);
            }
        }

        private void ProcessPacket(NetworkPacket packet, Socket source)
        {
            if (packet is JoinRequestPacket)
            {
                NetworkPacketSerializer.WritePacket(new WelcomePacket(Map), source);
                foreach (var player in Playground.Players)
                {
                    NetworkPacketSerializer.WritePacket(new PlayerSpawnPacket(player), source);
                }
                var newPlayer = Playground.SpawnNextPlayer();
                BroadcastPacket(new PlayerSpawnPacket(newPlayer));
                NetworkPacketSerializer.WritePacket(new AssignPlayerPacket(newPlayer.Id), source);
            }
            else if (packet is PlayerUpdatePacket)
            {
                var updatePacket = packet as PlayerUpdatePacket;
                Playground.Players.Single(p => p.Id == updatePacket.Id).Update(updatePacket.Position, updatePacket.Heading);
                BroadcastPacket(packet);
            }
        }

        private void BroadcastPacket(NetworkPacket packet)
        {
            foreach (var socket in ActiveSockets.Where(socket => socket != ListenSocket))
            {
                NetworkPacketSerializer.WritePacket(packet, socket);
            }
        }

        private void DropConnection(Socket socket)
        {
            ActiveSockets.Remove(socket);
            socket.Close();
        }
    }
}
