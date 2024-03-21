using System;
using System.Collections.Generic;
using System.Text;

namespace pwither.net.client.Enums
{
    public enum TunnelState
    {
        None = -1,
        Prepare,
        Handshake_Send,
        Handshake_Receive,
        Ack_Send,
        Ack_Receive,
        ResolveChallange,
        Establish,
        Connected
    }
}
