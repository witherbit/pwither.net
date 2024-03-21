using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pwither.net.server.SocketClients;
using pwither.net.Nodes;

namespace pwither.net.server.Objects
{
    public class Update
    {
        public Node Node { get; set; }
        public TcpSocketClient Client { get; set; }
    }
}
