using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Xml.Linq;
using pwither.net.server.Sockets;
using pwither.net.Nodes;
using pwither.net.server.Enums;
using pwither.net.server.Exceptions;
using TcpSocket = pwither.net.server.Sockets.TcpSocket;
using pwither.net.server.EV;

namespace pwither.net.server.SocketClients
{
    public class TcpSocketClient
    {
        public TcpClient TcpClient { get; set; }

        public NetworkStream Stream { get; set; }

        public string Ip
        {
            get;
        }

        public string ConnectionId { get; set; }

        public TcpSocket Instance { get; set; }

        public TcpSocketClient(TcpClient client, TcpSocket instance)
        {
            var time = DateTime.UtcNow.Ticks;
            var key = Guid.NewGuid().ToString();
            ConnectionId = Convert.ToBase64String(Encoding.UTF8.GetBytes(key + time));
            TcpClient = client;
            Instance = instance;

            IPEndPoint ipep = (IPEndPoint)TcpClient.Client.RemoteEndPoint;
            IPAddress ipa = ipep.Address;
            Ip = ipa.ToString();

            Instance.AddConnection(this);
        }

        internal void CloseInline()
        {
            if (Stream != null)
                Stream.Close();
            if (TcpClient != null)
                TcpClient.Close();
        }

        public void Close()
        {
            Instance.RemoveConnection(this);
        }

        public byte[] ReadStream()
        {
            var response = new List<byte>();
            do
            {
                response.Add((byte)Stream.ReadByte());
            }
            while (Stream != null && Stream.DataAvailable && !Instance.CancellationToken.IsCancellationRequested && TcpClient != null && TcpClient.Connected);


            return response.ToArray();
        }

        internal void ReceiveHandle()
        {
            try
            {
                Stream = TcpClient.GetStream();
                Receive();
            }
            catch (Exception ex)
            {
                TcpSocketDispatcher.Invoke(TcpSocketEventType.SocketMessage, Instance, Instance.GetSocketMessage.SetMessage($"Client exception on  {Ip} [{ConnectionId}]:").SetException(new TcpSocketException(ex.Message, ExceptionSocketCode.AnotherException, ex)));
            }
            finally
            {
                Close();
            }
        }

        public virtual void Receive()
        {
            while (!Instance.CancellationToken.IsCancellationRequested)
            {
                try
                {
                    var update = ReadStream();
                    TcpSocketDispatcher.Invoke(TcpSocketEventType.SocketMessage, Instance, new Objects.Update
                    {
                        Client = this,
                        Node = Node.Unpack(update)
                    });
                }
                catch (Exception ex)
                {
                    if (ex.Message.ConvertFromUTF8().ConvertToBase64() != "VW5leHBlY3RlZCBjaGFyYWN0ZXIgZW5jb3VudGVyZWQgd2hpbGUgcGFyc2luZyB2YWx1ZTog77+9LiBQYXRoICcnLCBsaW5lIDAsIHBvc2l0aW9uIDAu")
                        TcpSocketDispatcher.Invoke(TcpSocketEventType.SocketMessage, Instance, Instance.GetSocketMessage.SetMessage($"Client exception on {Ip} [{ConnectionId}]:").SetException(new TcpSocketException(ex.Message, ExceptionSocketCode.ClientReceiveException, ex)));
                    break;
                }
            }
        }

        public void Send(Node node)
        {
            Instance.Send(node, this);
        }
    }
}
