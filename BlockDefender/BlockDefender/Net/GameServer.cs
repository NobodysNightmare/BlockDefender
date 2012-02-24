﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.IO;
using BlockDefender.Net.Data;

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
            ListenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            ListenSocket.Bind(new IPEndPoint(IPAddress.Any, port));
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
            using (var stream = new NetworkStream(socket))
            {
                try
                {
                    NetworkPacket packet = NetworkPacketSerializer.ReadPacket(stream);
                    ProcessPacket(packet, stream);
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
        }

        private void ProcessPacket(NetworkPacket packet, NetworkStream source)
        {
            if (packet is JoinRequestPacket)
            {
                NetworkPacketSerializer.WritePacket(new WelcomePacket(Map), source);
                foreach (var player in Playground.Players)
                {
                    NetworkPacketSerializer.WritePacket(new PlayerSpawnPacket(player), source);
                }
                var newPlayer = Playground.SpawnNextPlayer();
                BoradcastPacket(new PlayerSpawnPacket(newPlayer));
                NetworkPacketSerializer.WritePacket(new AssignPlayerPacket(newPlayer.Id), source);
            }
        }

        private void BoradcastPacket(NetworkPacket packet)
        {
            foreach (var socket in ActiveSockets.Where(socket => socket != ListenSocket))
            {
                using (var stream = new NetworkStream(socket))
                {
                    NetworkPacketSerializer.WritePacket(packet, stream);
                }
            }
        }

        private void DropConnection(Socket socket)
        {
            ActiveSockets.Remove(socket);
            socket.Close();
        }
    }
}
