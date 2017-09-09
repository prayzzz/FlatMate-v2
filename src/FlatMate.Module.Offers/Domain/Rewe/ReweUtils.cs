using Microsoft.Extensions.Logging;
using prayzzz.Common.Attributes;
using System.Linq;

namespace FlatMate.Module.Offers.Domain.Rewe
{
    public interface IReweUtils
    {
        decimal ParsePrice(string price);

        string TrimDescription(string description);

        string TrimName(string name);
    }

    [Inject]
    public class ReweUtils : IReweUtils
    {
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

            description = description.Trim(',');
            return description;
        }

        public string TrimName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return name;
            }

            name = name.Trim('*');
            return name;
        }
    }
}
