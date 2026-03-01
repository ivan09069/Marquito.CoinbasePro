using Flurl.Http;
using Jose;
using Marquito.CoinbasePro.Client.Data.Account;
using Marquito.CoinbasePro.Client.Data.Order;
using Marquito.CoinbasePro.Client.Data.Permissions;
using Marquito.CoinbasePro.Client.Data.Product;
using Marquito.CoinbasePro.Enums.Extensions;
using Marquito.TradingBotUtils.Order.Exceptions;
using MarquitoUtils.Main.Currency.Tools;
using MarquitoUtils.Main.Http.Enums;
using MarquitoUtils.TradingAPI.Client;
using MarquitoUtils.TradingAPI.Entities.File;
using MarquitoUtils.TradingAPI.Enums;
using MarquitoUtils.TradingAPI.Enums.Extensions;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace Marquito.CoinbasePro.Client
{
    public class CoinbaseProClient : TradingClient
    {
        /// <summary>
        /// Coinbase trading client
        /// </summary>
        /// <param name="secretKey"><The secret key/param>
        /// <param name="organizationName">The organization name</param>
        /// <param name="apiKey">The token</param>
        public CoinbaseProClient(string secretKey, string organizationName, string apiKey)
            : base("api.coinbase.com/api/v3/brokerage", secretKey, organizationName, apiKey)
        {
            SecretKey = secretKey;
            OrganizationName = organizationName;
            ApiKey = apiKey;
        }

        /// <summary>
        /// Coinbase trading client
        /// </summary>
        /// <param name="tradingConfiguration">The trading configuration</param>
        public CoinbaseProClient(TradingConfiguration tradingConfiguration)
            : base("api.coinbase.com/api/v3/brokerage", tradingConfiguration)
        {

        }

        #region Commons methods

        /// <summary>
        /// Generate a JWT token for a specific resource and return it
        /// </summary>
        /// <param name="resource">A specific resource</param>
        /// <param name="httpMethod">The HTTP method type</param>
        /// <returns>A JWT token for a specific resource</returns>
        protected string GenerateToken(string resource, HttpMethodType httpMethod = HttpMethodType.GET)
        {
            string name = $"organizations/{OrganizationName}/apiKeys/{ApiKey}";

            byte[]? privateKeyBytes = Convert.FromBase64String(SecretKey);

            string? encodedToken;
            using (ECDsa key = ECDsa.Create())
            {

                key.ImportECPrivateKey(privateKeyBytes, out _);

                Dictionary<string, object> payload = new Dictionary<string, object>
                {
                    { "sub", name },
                    { "iss", "coinbase-cloud" },
                    { "nbf", Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds) },
                    { "exp", Convert.ToInt64((DateTime.UtcNow.AddMinutes(1) - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds) },
                    { "uri", $"{httpMethod.ToString()} {GetMainEndpoint()}/{resource}" },
                };

                Dictionary<string, object> extraHeaders = new Dictionary<string, object>
                {
                    { "kid", name },
                    // add nonce to prevent replay attacks with a random 10 digit number
                    { "nonce", RandomHex(10) },
                    { "typ", "JWT"},
                };

                encodedToken = JWT.Encode(payload, key, JwsAlgorithm.ES256, extraHeaders).Trim();
            }

            return encodedToken;
        }

        protected bool IsTokenValid(string token)
        {
            string tokenId = $"organizations/{OrganizationName}/apiKeys/{ApiKey}";
            if (token == null)
                return false;

            var key = ECDsa.Create();
            key?.ImportECPrivateKey(Convert.FromBase64String(SecretKey), out _);

            var securityKey = new ECDsaSecurityKey(key) { KeyId = tokenId };

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = securityKey,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out var validatedToken);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private string RandomHex(int digits)
        {
            Random random = new Random();
            byte[] buffer = new byte[digits / 2];
            random.NextBytes(buffer);
            string result = string.Concat(buffer.Select(x => x.ToString("X2")).ToArray());
            if (digits % 2 == 0)
                return result;
            return result + random.Next(16).ToString("X");
        }

        #endregion Commons methods

        #region Accounts

        /// <summary>
        /// Get accounts
        /// </summary>
        /// <returns>The list of accounts available</returns>
        public List<Account> GetAccounts()
        {
            string resource = $"accounts";

            IFlurlRequest request = GetMainUrl()
                .AppendPathSegment(resource)
                .WithOAuthBearerToken(GenerateToken(resource));

            return request.GetJsonAsync<AccountResponse>().Result.Accounts.Where(x => x.AccountBalance.Value > 0).ToList();
        }

        /// <summary>
        /// Get an account by it's ID
        /// </summary>
        /// <param name="accountID">The account's ID</param>
        /// <returns>An account by it's ID</returns>
        public Account GetAccount(Guid accountID)
        {
            string resource = $"accounts";

            IFlurlRequest request = GetMainUrl()
                .AppendPathSegment(resource)
                .AppendPathSegment(accountID.ToString())
                .WithOAuthBearerToken(GenerateToken(resource));

            return request.GetJsonAsync<Account>().Result;
        }

        /// <summary>
        /// Get an account by it's product currency
        /// </summary>
        /// <param name="productCurrency">The product currency</param>
        /// <returns>An account by it's product currency</returns>
        public Account? GetAccount(string productCurrency)
        {
            return this.RetrieveAccountWithCurrency(GetAccounts(), productCurrency);
        }

        private Account RetrieveAccountWithCurrency(List<Account> accounts, string productCurrency)
        {
            return accounts.Where(x => x.Currency == productCurrency).FirstOrDefault(new Account()
            {
                IsFakeAccount = true,
                AccountID = Guid.Empty,
                Name = $"{productCurrency} fake wallet",
                Currency = productCurrency,
                AccountBalance = new Data.Common.Balance()
                {
                    Value = 0,
                    Currency = productCurrency,
                },
                HoldBalance = new Data.Common.Balance()
                {
                    Value = 0,
                    Currency = productCurrency,
                },
            });
        }

        #endregion Accounts

        #region Products

        /// <summary>
        /// Get list of products
        /// </summary>
        /// <returns>Products</returns>
        public List<TradingProduct> GetProducts()
        {
            string resource = "products";

            IFlurlRequest request = GetMainUrl()
                .AppendPathSegment(resource)
                .WithOAuthBearerToken(GenerateToken(resource));

            return request.GetJsonAsync<TradingProductResponse>().Result.Products;
        }

        /// <summary>
        /// Get list of products
        /// </summary>
        /// <returns>Products</returns>
        public List<TradingProduct> GetProducts(List<string> products)
        {
            string resource = "products";

            IFlurlRequest request = GetMainUrl()
                .AppendPathSegment(resource)
                .SetQueryParam("product_ids", products)
                .WithOAuthBearerToken(GenerateToken(resource));

            return request.GetJsonAsync<TradingProductResponse>().Result.Products;
        }

        /// <summary>
        /// Get product
        /// </summary>
        /// <returns>Product</returns>
        public TradingProduct GetProduct(string product)
        {
            string resource = $"products/{product}";

            IFlurlRequest request = GetMainUrl()
                .AppendPathSegment(resource)
                .WithOAuthBearerToken(GenerateToken(resource));

            return request.GetJsonAsync<TradingProduct>().Result;
        }

        /// <summary>
        /// Get candle data (historical data) for a specific product
        /// </summary>
        /// <param name="product">The product</param>
        /// <param name="start">The start date</param>
        /// <param name="end">The end date</param>
        /// <param name="period">The period</param>
        /// <returns>Candle data (historical data) for a specific product</returns>
        public List<Candle> GetCandles(string product, DateTime start, DateTime end, TradingPeriod period)
        {
            string resource = $"products/{product}/candles";

            IFlurlRequest request = GetMainUrl()
                .AppendPathSegment(resource)
                .SetQueryParam("start", new DateTimeOffset(start).ToUnixTimeSeconds())
                .SetQueryParam("end", new DateTimeOffset(end).ToUnixTimeSeconds())
                .SetQueryParam("granularity", period.GetPeriodValue())
                .WithOAuthBearerToken(GenerateToken(resource));

            return request.GetJsonAsync<CandleResponse>().Result.Candles
                .OrderBy(x => x.Start).ToList();
        }

        /// <summary>
        /// Get candle data (historical data) for a specific product
        /// </summary>
        /// <param name="product">The product</param>
        /// <param name="period">The period</param>
        /// <param name="candleCount">The amount of candle (max 300 candles per request)</param>
        /// <returns>Candle data (historical data) for a specific product</returns>
        public List<Candle> GetCandles(string product, TradingPeriod period, short candleCount = 300)
        {
            DateTime now = DateTime.Now;

            long seconds = period.GetPeriodInSeconds() * candleCount;

            return GetCandles(product, now.AddSeconds(seconds * -1), now, period);
        }

        /// <summary>
        /// Get last market trade price for a specific product
        /// </summary>
        /// <param name="product">The product</param>
        /// <returns>The last market trade price for a specific product</returns>
        public virtual MarketTrade GetLastMarketTrade(string product)
        {
            string resource = $"products/{product}/ticker";

            IFlurlRequest request = GetMainUrl()
                .AppendPathSegment(resource)
                .SetQueryParam("limit", 1)
                .WithOAuthBearerToken(GenerateToken(resource));

            return request.GetJsonAsync<MarketTradeResponse>().Result.Trades.First();
        }

        #endregion Products

        #region Price conversions

        public double GetConvertedPrice(TradingProduct product, string fiatProductID, double valueToConvert)
        {
            return this.GetConvertedPrice(product.ProductID, fiatProductID, valueToConvert);
        }

        public double GetConvertedPrice(string productID, string fiatProductID, double valueToConvert)
        {
            string[] fiats = fiatProductID.Split("-");

            TradingProduct fiatProduct;
            if (productID.Contains(fiats[0]))
            {
                // Example : productID = BTC-USD, fiatProduct = USD-EUR
                // We convert from crypto to fiat
                fiatProduct = this.GetProduct($"{productID.Split("-")[1]}-{fiats[1]}");

                return (double)((decimal)valueToConvert * (decimal)fiatProduct.Price.Value);
            }
            else
            {
                // Example : productID = BTC-USD, fiatProduct = EUR-USD
                // We convert from fiat to crypto
                fiatProduct = this.GetProduct($"{productID.Split("-")[1]}-{fiats[0]}");

                return (double)((decimal)valueToConvert / (decimal)fiatProduct.Price.Value);
            }
        }

        #endregion Price conversions

        #region Orders

        /// <summary>
        /// Buy or sell crypto between two currencies of a product
        /// </summary>
        /// <param name="fromAccount">The first part of the product</param>
        /// <param name="targetAccount">The second part of the product</param>
        /// <param name="amountToConvert">The amount to convert between the pair</param>
        /// <param name="side">The side : we want to buy or sell ?</param>
        /// <returns></returns>
        /// <exception cref="NoFundsAvailableException">An account don't have enough funds to convert an amount of crypto.</exception>
        public void CanBuyOrSell(Account fromAccount, Account targetAccount, double amountToConvert, TradingSide side)
        {
            AccountPermissions accountPermissions = GetAccountPermissions();
            if (!accountPermissions.CanTrade)
            {
                throw new ApiKeyTradeRightException();
            }
            if (side == TradingSide.SELL && fromAccount.AccountBalance.Value < amountToConvert)
            {
                throw new NoFundsAvailableException(fromAccount.AccountID, fromAccount.Currency, $"{fromAccount.Currency}-{targetAccount.Currency}", side);
            }
            else if (side == TradingSide.BUY && targetAccount.AccountBalance.Value < amountToConvert)
            {
                throw new NoFundsAvailableException(targetAccount.AccountID, targetAccount.Currency, $"{fromAccount.Currency}-{targetAccount.Currency}", side);
            }
        }

        /// <summary>
        /// Buy or sell crypto between two currencies of a product
        /// </summary>
        /// <param name="productID">The product pair</param>
        /// <param name="amountToConvert">The amount to convert between the pair</param>
        /// <param name="side">The side : we want to buy or sell ?</param>
        /// <returns></returns>
        /// <exception cref="NoFundsAvailableException">An account don't have enough funds to convert an amount of crypto.</exception>
        public Order ConvertCrypto(Account fromAccount, Account targetAccount, double amountToConvert, TradingSide side)
        {
            // TODO Add buy / sell to user watch for more security

            string resource = "orders";

            // Check if the user has the right to trade, and if the accounts have enough funds to convert the amount
            CanBuyOrSell(fromAccount, targetAccount, amountToConvert, side);
            // Get trading product used to convert crypto
            TradingProduct tradingProduct = this.GetProduct($"{fromAccount.Currency}-{targetAccount.Currency}");
            // Get the correct amount, rounded down by base size, to convert based on the base increment of the trading product
            double amountBaseSized;
            // Construct market ioc
            object marketIoc;
            if (side == TradingSide.SELL)
            {
                amountBaseSized = CurrencyUtils.GetSizedAmount(amountToConvert, tradingProduct.BaseIncrement);
                // Sell side
                marketIoc = new
                {
                    base_size = amountBaseSized.ToString(CultureInfo.InvariantCulture),
                };
            }
            else
            {
                amountBaseSized = CurrencyUtils.GetSizedAmount(amountToConvert, tradingProduct.QuoteIncrement);
                // Buy side
                marketIoc = new
                {
                    quote_size = amountBaseSized.ToString(CultureInfo.InvariantCulture),
                };
            }

            // Create the request to convert crypto
            IFlurlRequest request = GetMainUrl()
                .AppendPathSegment(resource)
                .WithOAuthBearerToken(GenerateToken(resource, HttpMethodType.POST));
            // Get the response from the request
            OrderResponse orderResponse = request.PostJsonAsync(
                new
                {
                    client_order_id = Guid.NewGuid().ToString(),
                    product_id = $"{fromAccount.Currency}-{targetAccount.Currency}",
                    side = side.ToString(),
                    order_configuration = new
                    {
                        market_market_ioc = marketIoc,
                    },
                }).ReceiveJson<OrderResponse>().Result;

            if (!orderResponse.IsSuccess)
            {
                // Order failed, throw an exception with the error details
                throw new OrderException($"{fromAccount.Currency}-{targetAccount.Currency}", side, orderResponse.Error);
            }
            else
            {
                // Order succeeded, return the order
                return orderResponse.Order;
            }
        }

        public Order ConvertCrypto(string productID, double amountToConvert, TradingSide side)
        {
            List<Account> accounts = GetAccounts();

            Account fromAccount = this.RetrieveAccountWithCurrency(accounts, productID.Split('-')[0]);
            Account targetAccount = this.RetrieveAccountWithCurrency(accounts, productID.Split('-')[1]);

            return ConvertCrypto(fromAccount, targetAccount, amountToConvert, side);
        }

        public Order ConvertAllCrypto(Account fromAccount, Account targetAccount, TradingSide side)
        {
            double amount = 0;

            if (side == TradingSide.SELL && fromAccount != null)
            {
                amount = fromAccount.AccountBalance.Value;
            }
            else if (side == TradingSide.BUY && targetAccount != null)
            {
                amount = targetAccount.AccountBalance.Value;
            }

            return ConvertCrypto(fromAccount, targetAccount, amount, side);
        }

        public Order ConvertAllCrypto(string productID, TradingSide side)
        {
            List<Account> accounts = GetAccounts();

            Account fromAccount = this.RetrieveAccountWithCurrency(accounts, productID.Split('-')[0]);
            Account targetAccount = this.RetrieveAccountWithCurrency(accounts, productID.Split('-')[1]);

            return ConvertAllCrypto(fromAccount, targetAccount, side);
        }

        #endregion Orders

        #region Permissions

        /// <summary>
        /// Get permissions for your API key
        /// </summary>
        /// <returns>Permissions for your API key</returns>
        public virtual AccountPermissions GetAccountPermissions()
        {
            string resource = "key_permissions";

            IFlurlRequest request = GetMainUrl()
                .AppendPathSegment(resource)
                .WithOAuthBearerToken(GenerateToken(resource));

            return request.GetJsonAsync<AccountPermissions>().Result;
        }

        #endregion Permissions
    }
}
