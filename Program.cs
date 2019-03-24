using System;
using System.Net;
using System.Net.Sockets;

namespace battering_ram
{
    public static class Program
    {
        private static QueuePump<State> pump = new QueuePump<State>();
        private static IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 0);
        private static IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 61503);
        private static byte[] payload = new byte[packet_size];
        private static Random random = new Random();

        private const int max_sockets = 100;
        private const int packet_size = 100;

        public static void Main(string[] args)
        {
            pump.Popped += Pump_Popped;
            random.NextBytes(payload);
            for (int i = 0; i < max_sockets; i++)
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                socket.Bind(localEndPoint);
                socket.BeginSendTo(payload, 0, payload.Length, SocketFlags.None, remoteEndPoint, new AsyncCallback(SendToCallback), socket);
            }
        }

        private static void Pump_Popped(object sender, State e)
        {
            e.EndSend();
            e.Dispose();
        }

        private static void SendToCallback(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            State state = new State(socket, ar);

            pump.Push(state);
            pump.Run();
        }

    }
}
