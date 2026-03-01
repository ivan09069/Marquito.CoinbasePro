using Newtonsoft.Json;

namespace Marquito.CoinbasePro.Client.Data.Common
{
    public class Balance
    {
        [JsonProperty("currency")]
        public string Currency { get; set; }
        [JsonProperty("value")]
        public double Value { get; set; }
    }
}
