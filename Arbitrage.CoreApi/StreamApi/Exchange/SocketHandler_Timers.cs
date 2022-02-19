using Arbitrage.CoreApi.Enums;
using Arbitrage.CoreApi.Models.Exchange;
using Arbitrage.CoreApi.StreamApi.Objects;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.CoreApi.StreamApi.Exchange
{
    public partial class SocketHandler
    {
        /* Spot Timers */
        public System.Timers.Timer OpportunitiesTimer { get; set; }
        public bool OpportunitiesTimerRunning { get; set; }

        private void ConstructSocketTimers()
        {
            /* Spot Ticker Timer */
            OpportunitiesTimer = new System.Timers.Timer();
            OpportunitiesTimer.Elapsed += new System.Timers.ElapsedEventHandler(OpportunitiesTimer_Action);
            OpportunitiesTimer.Interval = 3 * 1000;
            OpportunitiesTimer.Enabled = true;
        }

        public async void OpportunitiesTimer_Action(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                // Check Point
                if (OpportunitiesTimerRunning)
                {
                    return;
                }

                // Is Running
                OpportunitiesTimerRunning = true;

                // Stream Opportunities
                foreach (KeyValuePair<string, ExchangeTracker> kvp in AppCache.MarketTracker)
                {
                    try
                    {
                        await StreamOpportunityAsync(kvp.Value);
                    }
                    catch { }
                }
            }
            catch { }
            finally
            {
                // Is Running
                OpportunitiesTimerRunning = false;
            }
        }

        public async Task StreamOpportunityAsync(ExchangeTracker tracker)
        {
            StreamResponse<ExchangeTracker> streamResponse = new StreamResponse<ExchangeTracker>(ChannelName(StreamChannelType.TraceAll), tracker);
            byte[] streamBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(streamResponse));
            await SendToChannelAsync(streamResponse.Channel, streamBytes);
        }
    }
}