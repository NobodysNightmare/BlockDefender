using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace BlockDefender.Networking
{
    class GameServer
    {
        private Thread ServerThread;
        private Playground Map;
        private Socket ListenSocket;
        private List<Socket> ActiveSockets;

        public GameServer(Playground map)
        {
            Map = map;
            ActiveSockets = new List<Socket>();
            ListenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IPv4);
            ListenSocket.Bind(new IPEndPoint(IPAddress.Any, AppSettings.Default.ListenPort));
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
            try
            {
                while (true)
                {
                    RunSingleLoop();
                }
            }
            catch (ThreadAbortException)
            {
                //TODO: clean up
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
                    NetworkPacket packet = NetworkPacketFactory.ReadPacket(stream);
                    ProcessPacket(packet);
                }
                catch (UnsupportedPacketException)
                {
                    DropConnection(socket);
                }
            }
        }

        private void ProcessPacket(NetworkPacket packet)
        {
            throw new NotImplementedException();
        }

        private void DropConnection(Socket socket)
        {
            ActiveSockets.Remove(socket);
            socket.Close();
        }
    }
}
