using Marquito.CoinbasePro.Client.Data.Common;
using Newtonsoft.Json;

namespace Marquito.CoinbasePro.Client.Data.Convert
{
    public class ConvertTrade
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("user_entered_amount")]
        public Balance UserEnteredAmount { get; set; }
        [JsonProperty("amount")]
        public Balance Amount { get; set; }
        [JsonProperty("subtotal")]
        public Balance SubTotal { get; set; }
        [JsonProperty("total")]
        public Balance Total { get; set; }
    }
}
