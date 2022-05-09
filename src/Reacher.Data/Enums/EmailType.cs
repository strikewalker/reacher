namespace Reacher.Data.Enums;
public enum EmailType
{
    New = 0,
    Failed = 1,
    InboundReach = 2,
    InboundForward = 3,
    PaymentRequest = 4,
    OutboundReply = 5,
    OutboundForward = 6,
    TooSoon = 7,
    PaymentSuccess = 8,
    Disabled = 9,
    TooBig = 10,
    Whitelisted = 11
}
