using System;
using Microsoft.Extensions.Logging;
using prayzzz.Common.Attributes;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FlatMate.Module.Offers.Domain.Adapter.Rewe
{
    public interface IReweUtils
    {
        decimal ParsePrice(string price);

        string Trim(string str);
    }

    [Inject]
    public class ReweUtils : IReweUtils
    {
        private const string Comma = ",";
        private const string DecimalPoint = ".";

        private static readonly CultureInfo DecimalCulture = new CultureInfo("en-US");
        private static readonly char[] TrimChars = { ' ', '*', ',', '(', ')' };
        private static readonly Regex TwoOrMoreWhitespaces = new Regex("[ ]{2,}");

        private readonly ILogger<ReweUtils> _logger;

        public ReweUtils(ILogger<ReweUtils> logger)
        {
            _logger = logger;
        }

        /// <summary>
        ///     Converts the string to a double value.
        ///     If no decimalpoint is present, it will be inserted.
        /// </summary>
        /// <returns>Price as dobule or <see cref="ReweConstants.DefaultPrice" />, if parsing fails.</returns>
        public decimal ParsePrice(string price)
        {
            if (string.IsNullOrEmpty(price))
            {
                return ReweConstants.DefaultPrice;
            }

            price = price.Replace(Comma, DecimalPoint);

            // if decimal point exists, parse now
            if (price.Contains(DecimalPoint))
            {
                return ParsePriceOrDefault(price);
            }

            // add leading zero to prices below 1€
            if (price.Length == 2)
            {
                price = "0" + price;
            }

            // add decimal point
            price = price.Insert(price.Length - 2, DecimalPoint);
            return ParsePriceOrDefault(price);

            // returns DefaultPrice, if price couldn't be parsed
            decimal ParsePriceOrDefault(string p)
            {
                if (decimal.TryParse(p, NumberStyles.Currency, DecimalCulture, out var parsedPrice))
                {
                    return parsedPrice;
                }

                _logger.LogWarning("Couldn't parse price '{price}'", p);
                return ReweConstants.DefaultPrice;
            }
        }

        public string Trim(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            str = str.Trim(TrimChars);
            str = str.Replace("\r\n", " ");
            str = str.Replace("\n", " ");
            str = TwoOrMoreWhitespaces.Replace(str, " ");

            return str;
        }
    }
}
