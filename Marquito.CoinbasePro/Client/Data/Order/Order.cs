using Newtonsoft.Json;

namespace Marquito.CoinbasePro.Client.Data.Order
{
    public class Order
    {
        [JsonProperty("order_id")]
        public string OrderID { get; set; }
        [JsonProperty("product_id")]
        public string ProductID { get; set; }
        [JsonProperty("side")]
        public string Side { get; set; }
        [JsonProperty("client_order_id")]
        public string ClientOrderID { get; set; }
    }
}
