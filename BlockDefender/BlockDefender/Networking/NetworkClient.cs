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

        public NetworkClient()
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            socket.Connect(AppSettings.Default.ConnectHost, AppSettings.Default.ConnectPort);
            Stream = new NetworkStream(socket, true);
        }

        public Playground EstablishConnection()
        {
            NetworkPacketSerializer.WritePacket(new JoinPacket(), Stream);
            var packet = NetworkPacketSerializer.ReadPacket(Stream);
            var welcome = packet as WelcomePacket;
            if (packet != null)
                return new Playground(welcome.Map);
            throw new Exception("Expected a welcome packet but received something else!");
        }

        public void Dispose()
        {
            Stream.Dispose();
        }
    }
}
