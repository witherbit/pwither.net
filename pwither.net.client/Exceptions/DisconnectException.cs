using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using pwither.net.client.Enums;

namespace pwither.net.client.Exceptions
{
    public class DisconnectException : Exception
    {
        public DisconnectionCode DisconnectCode { get; set; }

        public DisconnectException(string message, DisconnectionCode disconnectCode) : base(message)
        {
            DisconnectCode = disconnectCode;
        }
        public DisconnectException(string message, DisconnectionCode disconnectCode, Exception ex) : base(message, ex)
        {
            DisconnectCode = disconnectCode;
        }
    }
}
