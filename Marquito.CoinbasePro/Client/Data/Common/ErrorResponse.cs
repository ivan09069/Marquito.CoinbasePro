using MarquitoUtils.Main.Common.Tools;
using MarquitoUtils.TradingAPI.Client.Data.Common;
using Newtonsoft.Json;

namespace Marquito.CoinbasePro.Client.Data.Common
{
    /// <summary>
    /// Error response from the Coinbase Pro API
    /// </summary>
    public class ErrorResponse : IErrorResponse
    {
        [JsonProperty("error")]
        public string ErrorCode { get; set; }
        [JsonProperty("message")]
        public string ErrorMessage { get; set; }
        [JsonProperty("error_details")]
        public string ErrorDetails { get; set; }
        [JsonIgnore]
        public Dictionary<string, object> ErrorData { get; set; }

        public T GetData<T>(string key)
        {
            return (T)Utils.Nvl(this.ErrorData)[key];
        }
    }
}
