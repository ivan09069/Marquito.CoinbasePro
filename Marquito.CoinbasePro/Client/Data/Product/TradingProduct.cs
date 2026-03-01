using Newtonsoft.Json;
//using System.Diagnostics;

namespace Marquito.CoinbasePro.Client.Data.Product
{
    //[DebuggerDisplay("Product ID = {ProductID}")]
    public class TradingProduct
    {
        [JsonProperty("product_id")]
        public string ProductID { get; set; }
        [JsonProperty("price")]
        public double? Price { get; set; }
        [JsonProperty("base_currency_id")]
        public string CryptoID { get; set; }
        [JsonProperty("base_name")]
        public string CryptoName { get; set; }
        [JsonProperty("quote_currency_id")]
        public string CurrencyID { get; set; }
        [JsonProperty("quote_name")]
        public string CurrencyName { get; set; }
        [JsonProperty("base_increment")]
        public double BaseIncrement { get; set; }
        [JsonProperty("quote_increment")]
        public double QuoteIncrement { get; set; }

        public string GetCryptoPart()
        {
            return this.ProductID.Split("-")[0];
        }

        public string GetFiatPart()
        {
            return this.ProductID.Split("-")[1];
        }
    }
}
