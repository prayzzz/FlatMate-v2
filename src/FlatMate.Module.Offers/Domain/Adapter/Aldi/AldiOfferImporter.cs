using prayzzz.Common.Attributes;
using FlatMate.Module.Common;
using FlatMate.Module.Common.Extensions;
using FlatMate.Module.Offers.Domain.Adapter.Aldi.Jso;
using Microsoft.Extensions.Logging;
using prayzzz.Common.Results;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FlatMate.Module.Offers.Domain.Adapter.Aldi
{
    [Inject]
    public class AldiOfferImporter : OfferImporter
    {
        private readonly IAldiApi _aldiApi;
        private readonly IAldiUtils _aldiUtils;
        private readonly IRawOfferDataService _rawOfferService;

        public AldiOfferImporter(OffersDbContext dbContext,
                                 IAldiApi aldiApi,
                                 IAldiUtils aldiUtils,
                                 IRawOfferDataService rawOfferService,
                                 ILogger<AldiOfferImporter> logger) : base(dbContext, logger)
        {
            _aldiApi = aldiApi;
            _aldiUtils = aldiUtils;
            _rawOfferService = rawOfferService;
        }

        public override Company Company => Company.AldiNord;

        public override async Task<(Result, IEnumerable<Offer>)> ImportOffersFromApi(Market market)
        {
            var offerChunks = new List<Data>();
            try
            {
                var areas = XmlConvert.Deserialize<Data>(await _aldiApi.GetAreas());
                if (areas == null)
                {
                    return (new ErrorResult(ErrorType.InternalError, "Aldi Api error"), null);
                }

                foreach (var teaser in areas.Area.SelectMany(a => a.Teasers.Teaser))
                {
                    offerChunks.Add(XmlConvert.Deserialize<Data>(await _aldiApi.GetOffers(teaser.Catrel, teaser.Teaserxml)));
                }
            }
            catch (Exception e)
            {
                Logger.LogWarning(0, e, $"Error while requesting {nameof(IAldiApi)}");
                return (new ErrorResult(ErrorType.InternalError, $"{nameof(IAldiApi)} nicht verfügbar."), null);
            }

            var articles = offerChunks.SelectMany(o => o.Area.SelectMany(a => a.Articles.Article)).ToList();

            var (result, _) = await _rawOfferService.Save(XmlConvert.Serialize(articles), market.CompanyId);
            if (result.IsError)
            {
                Logger.LogWarning(0, "Saving raw offer data failed");
            }

            return await ProcessOffers(articles, market);
        }

        public override Task<(Result, IEnumerable<Offer>)> ImportOffersFromRaw(Market market, string data)
        {
            var articles = XmlConvert.Deserialize<List<Article>>(data);

            Logger.LogInformation($"Importing {articles.Count} offers");

            return ProcessOffers(articles, market);
        }

        private OfferDuration GetOfferDuration(Article article)
        {
            var duration = new OfferDuration();

            var from = DateTimeOffset.FromUnixTimeSeconds(long.Parse(article.Pack_timestamp_actiondate)).DateTime;
            if (from.DayOfWeek == DayOfWeek.Sunday)
            {
                duration.From = from.GetNextWeekday(DayOfWeek.Monday);
            }
            else
            {
                duration.From = from;
            }

            duration.To = duration.From.GetNextWeekday(DayOfWeek.Sunday);

            return duration;
        }

        private OfferTemp PreprocessOffer(Article article, Market market)
        {
            var offerDuration = GetOfferDuration(article);

            var imageUrl = string.Empty;
            var img = article.Images.Img.FirstOrDefault();
            if (img != null)
            {
                imageUrl = "http://www.aldi-nord.de/" + img.Slider_normal;
            }

            return new OfferTemp
            {
                Brand = _aldiUtils.Trim(article.Producer),
                Description = _aldiUtils.StripHTML(article.Shorttext),
                ExternalOfferId = $"{((DateTimeOffset)offerDuration.From).ToUnixTimeSeconds()}_{article.Articleid}",
                ExternalProductCategory = string.Empty,
                ExternalProductCategoryId = article.Catrel,
                ExternalProductId = article.Articleid,
                ImageUrl = imageUrl,
                Market = market,
                Name = _aldiUtils.Trim(article.Title),
                OfferBasePrice = article.Price_calc,
                OfferedFrom = offerDuration.From,
                OfferedTo = offerDuration.To,
                OfferPrice = _aldiUtils.ParsePrice(article.Price),
                ProductCategory = ProductCategoryEnum.Other,
                RegularPrice = _aldiUtils.ParsePrice(article.Price_old),
                SizeInfo = article.Price_extra
            };
        }

        private async Task<(Result, IEnumerable<Offer>)> ProcessOffers(List<Article> articles, Market market)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var offers = new List<Offer>();
            foreach (var article in articles)
            {
                var preprocessedOffer = PreprocessOffer(article, market);
                preprocessedOffer.Product = CreateOrUpdateProduct(preprocessedOffer);

                var offerDbo = CreateOrUpdateOffer(preprocessedOffer);
                offers.Add(offerDbo);
            }

            var result = await DbContext.SaveChangesAsync();

            stopwatch.Stop();
            Logger.LogInformation($"Processed {articles.Count} offers in {stopwatch.ElapsedMilliseconds}ms");

            return (result, offers);
        }
    }
}
