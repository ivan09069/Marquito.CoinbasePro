using Newtonsoft.Json;

namespace Marquito.CoinbasePro.Client.Data.Product
{
    public class TradingProductResponse
    {
        [JsonProperty("products")]
        public List<TradingProduct> Products { get; set; }
    }
}
