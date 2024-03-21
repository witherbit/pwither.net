using System;
using System.Collections.Generic;
using System.Text;

namespace pwither.net.server.Enums
{
    public enum ExceptionSocketCode
    {
        DroppedWhenListening = 500,
        ErrorByteSend = 502,
        ErrorNodeSend = 503,
        ClientReceiveException = 407,
        AnotherException = 400,
        ClientNodeUnpack = 507
    }
}
