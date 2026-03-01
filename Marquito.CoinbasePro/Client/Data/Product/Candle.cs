using Newtonsoft.Json;
//using System.Diagnostics;

namespace Marquito.CoinbasePro.Client.Data.Product
{
    /// <summary>
    /// A candle (represent a price at a specific instant)
    /// </summary>
    //[DebuggerDisplay("High = {High}, Low = {Low}, Open = {Open}, Close = {Close}, Volume = {Volume}, Start = {Start}")]
    public class Candle
    {
        /// <summary>
        /// Start time
        /// </summary>
        [JsonProperty("start")]
        public long Start { get; set; }
        /// <summary>
        /// Low price
        /// </summary>
        [JsonProperty("low")]
        public double Low { get; set; }
        /// <summary>
        /// High price
        /// </summary>
        [JsonProperty("high")]
        public double High { get; set; }
        /// <summary>
        /// Open price
        /// </summary>
        [JsonProperty("open")]
        public double Open { get; set; }
        /// <summary>
        /// Close price
        /// </summary>
        [JsonProperty("close")]
        public double Close { get; set; }
        /// <summary>
        /// Volume time
        /// </summary>
        [JsonProperty("volume")]
        public double Volume { get; set; }
    }
}
