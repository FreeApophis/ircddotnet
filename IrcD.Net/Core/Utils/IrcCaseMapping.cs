using IrcD.Utils;

namespace IrcD
{
    public enum IrcCaseMapping
    {
        [EnumDescription("ascii")]
        Ascii,
        [EnumDescription("rfc1459")]
        Rfc1459,
        [EnumDescription("strict-rfc1459")]
        StrictRfc1459
    }
}