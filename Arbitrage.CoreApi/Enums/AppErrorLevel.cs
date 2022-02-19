using Gizza.Data.Attributes;

namespace Arbitrage.CoreApi.Enums
{
    public enum AppErrorLevel
    {
        //[Label("Unknown")]
        //Unknown = 0,

        [EnumLabel("Trace")]
        Trace = 1,

        [EnumLabel("Info")]
        Info = 2,

        [EnumLabel("Debug")]
        Debug = 3,

        [EnumLabel("Warning")]
        Warning = 4,

        [EnumLabel("Error")]
        Error = 5,

        [EnumLabel("Fatal")]
        Fatal = 6,
    }
}