using Marquito.CoinbasePro.Client;
using Marquito.CoinbasePro.Client.Data.Account;
using Marquito.CoinbasePro.Client.Data.Common;
using Marquito.CoinbasePro.Client.Data.Permissions;
using Marquito.CoinbasePro.Exceptions;
using Marquito.CoinbasePro.Tests.Enums;
using MarquitoUtils.TradingAPI.Entities.File;
using MarquitoUtils.TradingAPI.Enums;
using Moq;

namespace Marquito.CoinbasePro.Tests
{
    /// <summary>
    /// Unit tests for CoinbasePro advanced library
    /// </summary>
    public class CoinbaseProTests
    {
        /// <summary>
        /// Testing trading product
        /// </summary>
        private readonly string TestingProduct = "BTC-USDC";
        /// <summary>
        /// The client
        /// </summary>
        private CoinbaseProClient CoinbaseProClient { get; }
        public CoinbaseProTests()
        {
            Mock<CoinbaseProClient> coinbaseProClientMock = new Mock<CoinbaseProClient>(new TradingConfiguration());

            coinbaseProClientMock.Setup(x => x.GetAccountPermissions())
                .Returns(new AccountPermissions()
                {
                    CanTrade = true,
                });

            this.CoinbaseProClient = coinbaseProClientMock.Object;
        }

        /// <summary>
        /// Test accounts for a trading product can buy or sell, or not
        /// </summary>
        /// <param name="testCase">The account test case</param>
        /// <param name="amountToTrade">The amount to sell or buy</param>
        [Theory(DisplayName = nameof(Trading_AccountTest))]
        [InlineData(TradingAccountTestCase.SellWithEmptyAccount, 100)]
        [InlineData(TradingAccountTestCase.SellWithFundedAccount, 100)]
        [InlineData(TradingAccountTestCase.SellWithFundedAccountWithoutEnoughFunds, 101)]
        [InlineData(TradingAccountTestCase.BuyWithEmptyAccount, 100)]
        [InlineData(TradingAccountTestCase.BuyWithFundedAccount, 100)]
        [InlineData(TradingAccountTestCase.BuyWithFundedAccountWithoutEnoughFunds, 101)]
        public void Trading_AccountTest(TradingAccountTestCase testCase, double amountToTrade)
        {
            Account cryptoAccount;
            Account fiatAccount;
            switch (testCase)
            {
                case TradingAccountTestCase.SellWithEmptyAccount:
                    cryptoAccount = this.GetEmptyAccount(true);
                    fiatAccount = this.GetEmptyAccount(false);
                    Assert.Throws<NoFundsAvailableException>(() =>
                    {
                        this.CoinbaseProClient.CanBuyOrSell(cryptoAccount, fiatAccount, amountToTrade, TradingSide.SELL);
                    });
                    break;
                case TradingAccountTestCase.SellWithFundedAccount:
                    cryptoAccount = this.GetFundedAccount(true);
                    fiatAccount = this.GetEmptyAccount(false);

                    try
                    {
                        this.CoinbaseProClient.CanBuyOrSell(cryptoAccount, fiatAccount, amountToTrade, TradingSide.SELL);

                        Assert.True(true);
                    }
                    catch (NoFundsAvailableException ex)
                    {
                        Assert.Fail(ex.Message);
                    }
                    break;
                case TradingAccountTestCase.SellWithFundedAccountWithoutEnoughFunds:
                    cryptoAccount = this.GetFundedAccount(true);
                    fiatAccount = this.GetEmptyAccount(false);
                    Assert.Throws<NoFundsAvailableException>(() =>
                    {
                        this.CoinbaseProClient.CanBuyOrSell(cryptoAccount, fiatAccount, amountToTrade, TradingSide.SELL);
                    });
                    break;
                case TradingAccountTestCase.BuyWithEmptyAccount:
                    cryptoAccount = this.GetEmptyAccount(true);
                    fiatAccount = this.GetEmptyAccount(false);
                    Assert.Throws<NoFundsAvailableException>(() =>
                    {
                        this.CoinbaseProClient.CanBuyOrSell(cryptoAccount, fiatAccount, amountToTrade, TradingSide.BUY);
                    });
                    break;
                case TradingAccountTestCase.BuyWithFundedAccount:
                    cryptoAccount = this.GetEmptyAccount(true);
                    fiatAccount = this.GetFundedAccount(false);

                    try
                    {
                        this.CoinbaseProClient.CanBuyOrSell(cryptoAccount, fiatAccount, amountToTrade, TradingSide.BUY);

                        Assert.True(true);
                    }
                    catch (NoFundsAvailableException ex)
                    {
                        Assert.Fail(ex.Message);
                    }
                    break;
                case TradingAccountTestCase.BuyWithFundedAccountWithoutEnoughFunds:
                    cryptoAccount = this.GetEmptyAccount(true);
                    fiatAccount = this.GetFundedAccount(false);
                    Assert.Throws<NoFundsAvailableException>(() =>
                    {
                        this.CoinbaseProClient.CanBuyOrSell(cryptoAccount, fiatAccount, amountToTrade, TradingSide.BUY);
                    });
                    break;
                default:
                    Assert.Fail($"{testCase.ToString()} test case not managed");
                    break;
            }
        }

        #region Account methods

        /// <summary>
        /// Get empty account
        /// </summary>
        /// <param name="cryptoAccount">Crypto account or fiat account ?</param>
        /// <returns>Empty account</returns>
        private Account GetEmptyAccount(bool cryptoAccount)
        {
            return this.GetAccount(cryptoAccount, 0);
        }

        /// <summary>
        /// Get funded account
        /// </summary>
        /// <param name="cryptoAccount">Crypto account or fiat account ?</param>
        /// <returns>Ffunded account</returns>
        private Account GetFundedAccount(bool cryptoAccount)
        {
            return this.GetAccount(cryptoAccount, 100);
        }

        /// <summary>
        /// Get account
        /// </summary>
        /// <param name="cryptoAccount">Crypto account or fiat account ?</param>
        /// <param name="accountAmount">Amount of the account</param>
        /// <returns>Account</returns>
        private Account GetAccount(bool cryptoAccount, double accountAmount)
        {
            string currency = cryptoAccount ? this.TestingProduct.Split("-").First() : this.TestingProduct.Split("-").Last();
            return new Account()
            {
                Currency = currency,
                AccountBalance = new Balance()
                {
                    Currency = currency,
                    Value = accountAmount,
                },
            };
        }

        #endregion Account methods
    }
}