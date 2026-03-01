using System.Text.Json.Serialization;

namespace Marquito.CoinbasePro.Client.Data.Product
{
    /// <summary>
    /// The candle response object
    /// </summary>
    public class CandleResponse
    {
        /// <summary>
        /// A list of candles
        /// </summary>
        [JsonPropertyName("candles")]
        public List<Candle> Candles { get; set; }
    }
}
