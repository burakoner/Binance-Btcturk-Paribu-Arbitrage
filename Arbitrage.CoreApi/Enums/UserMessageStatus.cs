using Gizza.Data.Attributes;

namespace Arbitrage.CoreApi.Enums
{
    public enum UserMessageStatus
    {
        [EnumLabel("None")]
        None = 0,

        [EnumLabel("New")]
        New = 1,

        [EnumLabel("Sending")]
        Sending = 2,

        [EnumLabel("Sent")]
        Sent = 3,

        [EnumLabel("Failed")]
        Failed = 9,
    }
}