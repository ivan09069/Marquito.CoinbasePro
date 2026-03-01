using Marquito.CoinbasePro.Client.Data.Common;
using Newtonsoft.Json;
//using System.Diagnostics;

namespace Marquito.CoinbasePro.Client.Data.Account
{
    //[DebuggerDisplay("AccountID = {AccountID}, Name = {Name}, Currency = {Currency}, AccountBalance = {AccountBalance.Value}")]
    public class Account
    {
        [JsonProperty("uuid")]
        public Guid AccountID { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("currency")]
        public string Currency { get; set; }
        [JsonProperty("available_balance")]
        public Balance AccountBalance { get; set; }
        [JsonProperty("active")]
        public bool Active { get; set; }
        [JsonProperty("type")]
        public string CurrencyType { get; set; }
        [JsonProperty("hold")]
        public Balance HoldBalance { get; set; }
        [JsonProperty("retail_portfolio_id")]
        public string RetailPortfolioId { get; set; }
        [JsonIgnore]
        public bool IsFakeAccount { get; set; }
    }
}
