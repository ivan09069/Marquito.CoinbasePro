using Newtonsoft.Json;

namespace Marquito.CoinbasePro.Client.Data.Permissions
{
    public class AccountPermissions
    {
        [JsonProperty("can_view")]
        public bool CanView { get; set; }
        [JsonProperty("can_trade")]
        public bool CanTrade { get; set; }
        [JsonProperty("can_transfer")]
        public bool CanTransfer { get; set; }
        [JsonProperty("portfolio_uuid")]
        public string PortfolioUUID { get; set; }
        [JsonProperty("portfolio_type")]
        public string PortfolioType { get; set; }
    }
}
