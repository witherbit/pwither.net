using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using pwither.net.server.Exceptions;

namespace pwither.net.server.Objects
{
    public class SocketMessage
    {
        public string Name { get; set; }
        public string Message { get; set; }
        public TcpSocketException Exception { get; set; }
        public SocketMessage SetMessage(string message)
        {
            Message = message;
            return this;
        }
        public SocketMessage SetException(TcpSocketException ex)
        {
            Exception = ex;
            return this;
        }
    }
}
