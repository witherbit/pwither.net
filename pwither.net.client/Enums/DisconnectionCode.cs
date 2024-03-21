using System;
using System.Collections.Generic;
using System.Text;

namespace pwither.net.client.Enums
{
    public enum DisconnectionCode
    {
        Closed = -1,
        InvalidTag = -499,
        InvalidPeerNode = -498,
        InvalidChallenge = -495,
        InvalidMAC = -471,
        ChallengeVersionDifferent = -465,
        SignatureVerification = -464,
        InvalidEstablish = -496,
        InvalidAckResult = -463,
        InvalidAckVersionDifferent = -464,
        RejectionOfEstablishment = -401,
    }
}
