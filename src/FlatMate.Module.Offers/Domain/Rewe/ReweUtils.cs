using Microsoft.Extensions.Logging;
using prayzzz.Common.Attributes;
using System.Linq;

namespace FlatMate.Module.Offers.Domain.Rewe
{
    public interface IReweUtils
    {
        decimal ParsePrice(string price);

        string Trim(string str);
    }

    [Inject]
    public class ReweUtils : IReweUtils
    {
        private static readonly char[] TrimChars = new[] { ' ', '*', ',' };

        private readonly ILogger<ReweUtils> _logger;

        public ReweUtils(ILogger<ReweUtils> logger)
        {
            _logger = logger;
        }

        /// <summary>
        ///     Converts the string to a double value.
        ///     If no comma is present, it will be inserted.
        /// </summary>
        /// <returns>Price as dobule or <see cref="ReweConstants.DefaultPrice" /> if parsing fails.</returns>
        public decimal ParsePrice(string price)
        {
            if (string.IsNullOrEmpty(price))
            {
                return ReweConstants.DefaultPrice;
            }

            if (price.Contains(','))
            {
                return ParsePriceOrDefault(price);
            }

            // add leading zero to prices below 1€
            if (price.Length == 2)
            {
                price = "0" + price;
            }

            price = price.Insert(price.Length - 2, ",");
            return ParsePriceOrDefault(price);

            // returns DefaultPrice, if price couldn't be parsed
            decimal ParsePriceOrDefault(string p)
            {
                if (decimal.TryParse(p, out var parsedPrice))
                {
                    return parsedPrice;
                }

                _logger.LogWarning("Couldn't parse price '{price}'", p);
                return ReweConstants.DefaultPrice;
            }
        }

        public string TrimDescription(string description)
        {
            if (string.IsNullOrEmpty(description))
            {
                return description;
            }

            return description;
        }

        public string Trim(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            str = str.Trim(TrimChars);

            return str;
        }
    }
}
