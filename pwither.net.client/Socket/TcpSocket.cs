using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using pwither.net.client;
using pwither.net.client.Enums;
using pwither.net.client.EV;
using pwither.net.client.Exceptions;
using pwither.net.client.Objects;
using pwither.net.Nodes;
using ConnectionState = pwither.net.client.Enums.ConnectionState;

namespace pwither.net.client.Socket
{
    public class TcpSocket : IDisposable
    {
        private CancellationTokenSource _source { get; set; }
        public TcpClient TcpClient { get; private set; }

        public NetworkStream Stream { get; private set; }

        public CancellationToken CancellationToken { get; private set; }

        public SocketConfig ConnectArgs { get; private set; }

        public ConnectionState ConnectionState { get; private set; }

        public TcpSocket(SocketConfig args)
        {
            ConnectArgs = args;
            ConnectionState = ConnectionState.Disconnected;
        }

        public void Connect()
        {
            if (_source == null)
            {
                _source = new CancellationTokenSource();
                CancellationToken = _source.Token;
                TcpClient = new TcpClient();
                try
                {
                    TcpClient.Connect(ConnectArgs.Address, ConnectArgs.Port);
                    Stream = TcpClient.GetStream();
                    Thread receiveThread = new Thread(new ThreadStart(ReceiveHandle));
                    receiveThread.Start();
                }
                catch (Exception ex)
                {
                    Disconnect(true);
                }
            }
        }

        public void Disconnect(bool dropped = false, DisconnectException ex = null)
        {
            if (_source != null)
            {
                _source.Cancel();
                _source.Dispose();
                _source = null;
                if (Stream != null)
                    Stream.Close();
                if (TcpClient != null)
                    TcpClient.Close();
                ConnectionState = dropped ? ConnectionState.Dropped : ConnectionState.Disconnected;
                TcpSocketDispatcher.Invoke(TcpSocketEventType.ConnectionState, this, ConnectionState);
                TcpSocketDispatcher.Invoke(TcpSocketEventType.Disconnect, this, ex);
            }
        }

        public byte[] ReadStream()
        {
            var response = new List<byte>();
            do
            {
                response.Add((byte)Stream.ReadByte());
            }
            while (Stream.DataAvailable && !CancellationToken.IsCancellationRequested);

            return response.ToArray();
        }

        private void ReceiveHandle()
        {
            try
            {
                CheckConnection();
                Receive();
            }
            catch (Exception ex)
            {
                Disconnect(true);
            }
        }

        public virtual void Receive()
        {
            while (!CancellationToken.IsCancellationRequested)
            {
                try
                {
                    var update = ReadStream();
                    TcpSocketDispatcher.Invoke(TcpSocketEventType.Update, this, Node.Unpack(update));
                }
                catch (Exception ex)
                {
                    Disconnect(true);
                }
            }
        }

        private void Send(byte[] data)
        {
            try
            {
                if (_source != null)
                {

                    Stream.Write(data, 0, data.Length);
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void CheckConnection()
        {
            while (!TcpClient.Connected) ;
            ConnectionState = ConnectionState.Connected;
            TcpSocketDispatcher.Invoke(TcpSocketEventType.ConnectionState, this, ConnectionState);
        }

        public virtual void Send(Node node)
        {
            Send(node.Pack());
            TcpSocketDispatcher.Invoke(TcpSocketEventType.Update, this, node);
        }

        #region dispose
        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: освободить управляемое состояние (управляемые объекты)
                }

                // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить метод завершения
                // TODO: установить значение NULL для больших полей
                disposedValue = true;
            }
        }

        // // TODO: переопределить метод завершения, только если "Dispose(bool disposing)" содержит код для освобождения неуправляемых ресурсов
        // ~ClientConnector()
        // {
        //     // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
