using System;

namespace Arbitrage.CoreApi.StreamApi.Objects
{
    public class SocketSubscription
    {
        public string Channel { get; private set; }
        public DateTime SubscribedAt { get; private set; }
        public DateTime LastFeedTime { get; private set; }
        public TimeSpan FeedInterval { get; private set; }

        public SocketSubscription()
        {
            Channel = string.Empty;
            SubscribedAt = AppStatic.Now;
            LastFeedTime = AppStatic.Now;
            FeedInterval = TimeSpan.FromSeconds(60);
        }

        public SocketSubscription(string channel)
        {
            Channel = channel;
            SubscribedAt = AppStatic.Now;
            LastFeedTime = AppStatic.Now;
            FeedInterval = TimeSpan.FromSeconds(60);
        }
    }
}
