using System;
using System.Net.Sockets;

namespace battering_ram
{
    internal class State : IDisposable
    {
        public Socket _udpSocket;
        public IAsyncResult _asyncResult;

        public State(Socket socket, IAsyncResult ar)
        {
            _udpSocket = socket;
            _asyncResult = ar;
        }

        public void EndSend()
        {
            _udpSocket.EndSendTo(_asyncResult);
        }

        public void Dispose()
        {
            _udpSocket.Close();
        }
    }
}
