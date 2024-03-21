using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pwither.net.server.Enums
{
    public enum TcpSocketEventType
    {
        /// <summary>
        /// TcpSocket, Update
        /// </summary>
        ReceiveUpdate,
        /// <summary>
        /// TcpSocket, HostState
        /// </summary>
        StateChanged,
        /// <summary>
        /// TcpSocket, SocketMessage
        /// </summary>
        SocketMessage
    }
}
