using Gizza.Data.Attributes;

namespace Arbitrage.CoreApi.Enums
{
    public enum StreamOperationCode
    {
        None,

        [EnumLabel("ping")]
        Ping,

        [EnumLabel("pong")]
        Pong,

        [EnumLabel("login")]
        Login,

        [EnumLabel("logout")]
        Logout,

        [EnumLabel("subscribe")]
        Subscribe,

        [EnumLabel("unsubscribe")]
        Unsubscribe,

        [EnumLabel("subscriptions")]
        Subscriptions,
    }
}
