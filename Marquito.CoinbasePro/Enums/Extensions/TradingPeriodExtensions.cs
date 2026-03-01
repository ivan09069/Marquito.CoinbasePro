using MarquitoUtils.TradingAPI.Enums;

namespace Marquito.CoinbasePro.Enums.Extensions
{
    public static class TradingPeriodExtensions
    {
        public static string GetPeriodValue(this TradingPeriod period)
        {
            string periodValue;

            switch (period)
            {
                case TradingPeriod.M1:
                    periodValue = "ONE_MINUTE";
                    break;
                case TradingPeriod.M5:
                    periodValue = "FIVE_MINUTE";
                    break;
                case TradingPeriod.M15:
                    periodValue = "FIFTEEN_MINUTE";
                    break;
                case TradingPeriod.M30:
                    periodValue = "THIRTY_MINUTE";
                    break;
                case TradingPeriod.H1:
                    periodValue = "ONE_HOUR";
                    break;
                case TradingPeriod.H2:
                    periodValue = "TWO_HOUR";
                    break;
                case TradingPeriod.H6:
                    periodValue = "SIX_HOUR";
                    break;
                case TradingPeriod.D1:
                    periodValue = "ONE_DAY";
                    break;
                default:
                    periodValue = "ONE_DAY";
                    break;
            }

            return periodValue;
        }
    }
}
