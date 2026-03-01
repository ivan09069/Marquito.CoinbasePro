using Newtonsoft.Json;

namespace Marquito.CoinbasePro.Client.Data.Account
{
    public class AccountResponse
    {
        [JsonProperty("accounts")]
        public List<Account> Accounts { get; set; }
    }
}
