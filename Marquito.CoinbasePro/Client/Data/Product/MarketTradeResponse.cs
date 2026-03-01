namespace Marquito.CoinbasePro.Client.Data.Product
{
    /// <summary>
    /// The market trade response object
    /// </summary>
    public class MarketTradeResponse
    {
        /// <summary>
        /// A list of market trades
        /// </summary>
        public List<MarketTrade> Trades { get; set; }
    }
}
