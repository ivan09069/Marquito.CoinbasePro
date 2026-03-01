namespace Marquito.CoinbasePro.Tests.Enums
{
    /// <summary>
    /// Trading account test case for sell or buy crypto
    /// </summary>
    public enum TradingAccountTestCase
    {
        SellWithEmptyAccount,
        SellWithFundedAccount,
        SellWithFundedAccountWithoutEnoughFunds,
        BuyWithEmptyAccount,
        BuyWithFundedAccount,
        BuyWithFundedAccountWithoutEnoughFunds,
    }
}
