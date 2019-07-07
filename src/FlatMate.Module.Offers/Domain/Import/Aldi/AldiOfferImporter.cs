using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FlatMate.Module.Common;
using FlatMate.Module.Common.Extensions;
using FlatMate.Module.Offers.Domain.Companies;
using FlatMate.Module.Offers.Domain.Import.Aldi.Jso;
using FlatMate.Module.Offers.Domain.Markets;
using FlatMate.Module.Offers.Domain.Offers;
using FlatMate.Module.Offers.Domain.Products;
using Microsoft.Extensions.Logging;
using prayzzz.Common.Attributes;
using prayzzz.Common.Results;

namespace FlatMate.Module.Offers.Domain.Import.Aldi
{
    [Inject]
    public class AldiOfferImporter : OfferImporter
    {
        private readonly IAldiApi _aldiApi;
        private readonly IAldiUtils _aldiUtils;

        public AldiOfferImporter(OffersDbContext dbContext,
                                 IAldiApi aldiApi,
                                 IAldiUtils aldiUtils,
                                 ILogger<AldiOfferImporter> logger) : base(dbContext, logger)
        {
            _aldiApi = aldiApi;
            _aldiUtils = aldiUtils;
        }

        public override Company Company => Company.AldiNord;

        public override async Task<Result> ImportOffersFromApi(Market market)
        {
            var offerChunks = new List<Data>();
            try
            {
                var areas = XmlConvert.Deserialize<Data>(await _aldiApi.GetAreas());
                if (areas == null)
                {
                    return new Result(ErrorType.InternalError, "Aldi Api error");
                }

                foreach (var teaser in areas.Area.SelectMany(a => a.Teasers.Teaser))
                {
                    offerChunks.Add(XmlConvert.Deserialize<Data>(await _aldiApi.GetOffers(teaser.Catrel, teaser.Teaserxml)));
                }
            }
            catch (Exception e)
            {
                Logger.LogWarning(0, e, $"Error while requesting {nameof(IAldiApi)}");
                return new Result(ErrorType.InternalError, $"{nameof(IAldiApi)} nicht verfügbar.");
            }

            var articles = offerChunks.SelectMany(o => o.Area.SelectMany(a => a.Articles.Article)).ToList();

            return await ProcessOffers(market, articles);
        }

        private (Result, OfferDuration) GetOfferDuration(Article article)
        {
            var (result, from) = _aldiUtils.GetStartDateFromTitle(article);
            if (result.IsError)
            {
                Logger.LogWarning($"Cannot parse date from title {article.PackTitle}");
                return (result, null);
            }

            if (from.DayOfWeek == DayOfWeek.Sunday)
            {
                from = from.GetNextWeekday(DayOfWeek.Monday);
            }

            var to = from.GetNextWeekday(DayOfWeek.Sunday);

            return (Result.Success, new OfferDuration(from, to));
        }

        private (Result, OfferTemp) PreprocessOffer(Article article, Market market)
        {
            var (result, offerDuration) = GetOfferDuration(article);
            if (result.IsError)
            {
                return (result, null);
            }

            var imageUrl = string.Empty;
            var img = article.Images.Img.FirstOrDefault();
            if (img != null)
            {
                imageUrl = "http://www.aldi-nord.de/" + img.SliderNormal;
            }

            var offerTemp = new OfferTemp
            {
                BasePrice = _aldiUtils.Trim(article.PriceCalc),
                Brand = _aldiUtils.Trim(article.Producer),
                Company = Company,
                Description = _aldiUtils.StripHtml(article.Shorttext),
                ExternalOfferId = $"{((DateTimeOffset) offerDuration.From).ToUnixTimeSeconds()}_{article.Articleid}",
                ExternalProductCategory = string.Empty,
                ExternalProductCategoryId = article.Catrel,
                ImageUrl = imageUrl,
                Market = market,
                Name = _aldiUtils.Trim(article.Title),
                OfferedFrom = offerDuration.From,
                OfferedTo = offerDuration.To,
                OfferPrice = _aldiUtils.ParsePrice(article.Price),
                ProductCategory = ProductCategoryEnum.Other,
                RegularPrice = _aldiUtils.ParsePrice(article.PriceOld),
                SizeInfo = article.PriceExtra
            };

            return (Result.Success, offerTemp);
        }

        private async Task<Result> ProcessOffers(Market market, List<Article> articles)
        {
            var stopwatch = Stopwatch.StartNew();

            var offerTemp = new HashSet<OfferTemp>();
            foreach (var article in articles)
            {
                var (result, offer) = PreprocessOffer(article, market);
                if (result.IsSuccess)
                {
                    offerTemp.Add(offer);
                }
            }

            foreach (var o in offerTemp)
            {
                var product = CreateOrUpdateProduct(o);
                CreateOrUpdateOffer(o, product);
            }

            var saveResult = await DbContext.SaveChangesAsync();

            stopwatch.Stop();
            Logger.LogInformation($"Processed {articles.Count} offers in {stopwatch.Elapsed}");

            return saveResult;
        }
    }
}