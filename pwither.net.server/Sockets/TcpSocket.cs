using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using pwither.net.Nodes;
using pwither.net.server.Objects;
using pwither.net.server.SocketClients;
using pwither.net.server.EV;
using pwither.net.server.Enums;
using pwither.net.server.Exceptions;

namespace pwither.net.server.Sockets
{
    public class TcpSocket   
    {
        private CancellationTokenSource _source { get; set; }
        public CancellationToken CancellationToken { get; private set; }

        public SocketConfig Config { get; }

        private HostState _state { get; set; }
        public HostState State { 
            get => _state;
            private set
            {
                _state = value;
                TcpSocketDispatcher.Invoke(TcpSocketEventType.StateChanged, this, _state);
            }
        }

        public virtual SocketMessage GetSocketMessage => new SocketMessage { Name = Config.Name };

        public TcpListener TcpListener { get; private set; }

        private List<TcpSocketClient> _clients { get; set; }
        public TcpSocketClient[] Clients => _clients?.ToArray();
        public TcpSocket(SocketConfig config)
        {
            Config = config;
            _clients = new List<TcpSocketClient>();
            _state = HostState.Closed;
        }

        public virtual void Initialize()
        {
            TcpSocketDispatcher.Invoke(TcpSocketEventType.SocketMessage, this, GetSocketMessage.SetMessage($"The socket has been initialized on {Config.Port}"));
        }

        public virtual void AddConnection(TcpSocketClient client)
        {
            _clients.Add(client);

            TcpSocketDispatcher.Invoke(TcpSocketEventType.SocketMessage, this, GetSocketMessage.SetMessage($"A new connection has been registered from {client.Ip} [{client.ConnectionId}]"));
        }

        internal void CloseInline()
        {
            if (_source != null)
            {
                _source.Cancel();
                _source.Dispose();
                _source = null;
                TcpListener.Stop();
                for (int i = 0; i < _clients.Count; i++)
                {
                    _clients[i].CloseInline();
                }
                _clients.Clear();

                TcpSocketDispatcher.Invoke(TcpSocketEventType.SocketMessage, this, GetSocketMessage.SetMessage($"The socket was closed with the state: {_state}"));
            }
        }

        public void Close()
        {
            if (_source != null)
            {
                State = HostState.Closed;
                CloseInline();
            }
        }

        private void Listen()
        {
            if (_source != null)
            {
                try
                {
                    TcpListener = new TcpListener(IPAddress.Any, Config.Port);
                    TcpListener.Start();

                    TcpSocketDispatcher.Invoke(TcpSocketEventType.SocketMessage, this, GetSocketMessage.SetMessage($"The socket has started listening"));

                    while (!CancellationToken.IsCancellationRequested)
                    {
                        TcpClient tcpClient = TcpListener.AcceptTcpClient();
                        var clientObject = new TcpSocketClient(tcpClient, this);
                        Thread clientThread = new Thread(new ThreadStart(clientObject.ReceiveHandle));
                        clientThread.Start();
                    }
                    State = HostState.Closed;
                    CloseInline();
                }
                catch (Exception ex)
                {
                    State = HostState.Dropped;
                    CloseInline();

                    TcpSocketDispatcher.Invoke(TcpSocketEventType.SocketMessage, this, GetSocketMessage.SetMessage($"Socket exception").SetException(new TcpSocketException(ex.Message, ExceptionSocketCode.DroppedWhenListening, ex)));
                }
            }
        }

        public void Open()
        {
            if (_source == null)
            {
                _source = new CancellationTokenSource();
                CancellationToken = _source.Token;
                Thread receiveThread = new Thread(new ThreadStart(Listen));
                receiveThread.Start();
            }
        }

        public void RemoveConnection(string connectionId)
        {
            var client = _clients.FirstOrDefault(c => c.ConnectionId == connectionId);
            if (client != null)
            {
                TcpSocketDispatcher.Invoke(TcpSocketEventType.SocketMessage, this, GetSocketMessage.SetMessage($"The connection from {client.Ip} has been deleted [{client.ConnectionId}]"));

                client.CloseInline();
                _clients.Remove(client);
            }
        }
        public void RemoveConnection(TcpSocketClient client)
        {
            if (client != null)
            {
                TcpSocketDispatcher.Invoke(TcpSocketEventType.SocketMessage, this, GetSocketMessage.SetMessage($"The connection from {client.Ip} has been deleted [{client.ConnectionId}]"));

                client.CloseInline();
                _clients.Remove(client);
            }
        }

        public virtual void Send(Node node, string connectionId)
        {
            try
            {
                var client = _clients.FirstOrDefault(c => c.ConnectionId == connectionId);
                var data = node.Pack();
                if (client != null)
                {
                    client.Stream.Write(data, 0, data.Length);

                    TcpSocketDispatcher.Invoke(TcpSocketEventType.SocketMessage, this, GetSocketMessage.SetMessage($" Sent {data.Length} bytes to {client.Ip} [{client.ConnectionId}]\nNode instance:\n{node.ToString()}"));
                }
            }
            catch (Exception ex)
            {
                TcpSocketDispatcher.Invoke(TcpSocketEventType.SocketMessage, this, GetSocketMessage.SetMessage($"Socket exception").SetException(new TcpSocketException(ex.Message, ExceptionSocketCode.ErrorNodeSend, ex)));
            }
        }
        public virtual void Send(Node node, TcpSocketClient client)
        {
            try
            {
                var data = node.Pack();
                if (client != null)
                {
                    client.Stream.Write(data, 0, data.Length);

                    TcpSocketDispatcher.Invoke(TcpSocketEventType.SocketMessage, this, GetSocketMessage.SetMessage($" Sent {data.Length} bytes to {client.Ip} [{client.ConnectionId}]\nNode instance:\n{node.ToString()}"));
                }
            }
            catch (Exception ex)
            {
                TcpSocketDispatcher.Invoke(TcpSocketEventType.SocketMessage, this, GetSocketMessage.SetMessage($"Socket exception").SetException(new TcpSocketException(ex.Message, ExceptionSocketCode.ErrorNodeSend, ex)));
            }
        }
    }
}
