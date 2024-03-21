using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pwither.net.client.Enums
{
    public enum TcpSocketEventType
    {
        /// <summary>
        /// TcpSocket, Node
        /// </summary>
        Update,
        /// <summary>
        /// TcpSocket, ConnectionState
        /// </summary>
        ConnectionState,
        /// <summary>
        /// TcpSocket, DisconnectException
        /// </summary>
        Disconnect,
        /// <summary>
        /// TcpSocket, Node
        /// </summary>
        Send
    }
}
