using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pwither.net.server.Enums;

namespace pwither.net.server.Exceptions
{
    public class TcpSocketException : Exception
    {
        public ExceptionSocketCode ExceptionSocketCode { get; set; }

        public TcpSocketException(string message, ExceptionSocketCode disconnectCode) : base(message)
        {
            ExceptionSocketCode = disconnectCode;
        }
        public TcpSocketException(string message, ExceptionSocketCode disconnectCode, Exception ex) : base(message, ex)
        {
            ExceptionSocketCode = disconnectCode;
        }
    }
}
