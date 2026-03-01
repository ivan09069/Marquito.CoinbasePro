using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Marquito.CoinbasePro.Client.Data.Product
{
    /// <summary>
    /// A market trade (represent the current price of a product
    /// </summary>
    public class MarketTrade
    {
        /// <summary>
        /// The time
        /// </summary>
        [JsonProperty("time")]
        public DateTimeOffset Time { get; set; }
        /// <summary>
        /// The product ID
        /// </summary>
        [JsonProperty("product_id")]
        public string ProductID { get; set; }
        /// <summary>
        /// The current price
        /// </summary>
        [JsonProperty("price")]
        public double Price { get; set; }
    }
}
