using System;
using System.Globalization;
using System.Text.RegularExpressions;
using FlatMate.Module.Offers.Domain.Adapter.Aldi.Jso;
using Microsoft.Extensions.Logging;
using prayzzz.Common.Attributes;
using prayzzz.Common.Results;

namespace FlatMate.Module.Offers.Domain.Adapter.Aldi
{
    public interface IAldiUtils
    {
        (Result, DateTime) GetStartDateFromTitle(Article article);

        decimal ParsePrice(string price);

        string StripHtml(string str);

        string Trim(string str);
    }

    [Inject]
    public class AldiUtils : IAldiUtils
    {
        private const string Comma = ",";
        private const string Hyphen = "-";
        private const string OtherHyphen = "–";
        private const string DecimalPoint = ".";

        private static readonly CultureInfo DecimalCulture = new CultureInfo("en-US");
        private static readonly char[] TrimChars = { ' ', '*', ',', '.' };
        private static readonly Regex TwoOrMoreWhitespaces = new Regex("[ ]{2,}");
        private static readonly Regex TitleWithDate = new Regex(@"^(.*)(\d{2}\.\d{2}.)$");
        private static readonly string DatePattern = "dd.MM.";

        private readonly ILogger<AldiUtils> _logger;

        public AldiUtils(ILogger<AldiUtils> logger)
        {
            _logger = logger;
        }

        public (Result, DateTime) GetStartDateFromTitle(Article article)
        {
            var match = TitleWithDate.Match(article.PackTitle);
            if (match.Success)
            {
                var date = match.Groups[2].Value;
                if (DateTime.TryParseExact(date, DatePattern, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                {
                    return (Result.Success, dt);
                }
            }

            return (new Result(ErrorType.ValidationError, ""), DateTime.MinValue);
        }

        public decimal ParsePrice(string price)
        {
            if (string.IsNullOrEmpty(price))
            {
                return AldiConstants.DefaultPrice;
            }

            // fix 123,45
            price = price.Replace(Comma, DecimalPoint);

            // fix 123.-
            price = price.Replace(Hyphen, string.Empty);
            price = price.Replace(OtherHyphen, string.Empty);

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
                return AldiConstants.DefaultPrice;
            }
        }

        public string StripHtml(string str)
        {
            str = str.Replace("<li>", " ");
            return Trim(Regex.Replace(str, "<.*?>", string.Empty));
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
            str = Regex.Replace(str, "^EW\\s", string.Empty);
            str = Regex.Replace(str, "^MW\\s", string.Empty);
            str = Regex.Replace(str, "^L\\s", string.Empty);
            str = TwoOrMoreWhitespaces.Replace(str, " ");

            return str;
        }
    }
}