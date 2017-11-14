﻿using FlatMate.Module.Common.Extensions;
using FlatMate.Module.Offers.Domain;
using FlatMate.Module.Offers.Domain.Adapter.Rewe;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using prayzzz.Common.Results;
using prayzzz.Common.Unit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlatMate.Module.Offers.Test.Rewe
{
    [TestClass]
    public class ReweOfferImporterTest
    {
        private const string MarketId = "123465";

        /// <summary>
        ///     Test with a large dataset
        /// </summary>
        [TestMethod]
        public async Task LoadOffers_Large()
        {
            var dbContext = new OffersDbContext(new DbContextOptionsBuilder<OffersDbContext>().UseInMemoryDatabase("LoadOffers_Large").Options,
                                                new ConsoleLogger<OffersDbContext>());

            var rawOfferMock = TestHelper.Mock<IRawOfferDataService>();
            rawOfferMock.Setup(x => x.Save(It.IsAny<string>(), It.IsAny<int>())).Returns(Task.FromResult(((Result)SuccessResult.Default, new RawOfferDataDto())));

            var mobileApiMock = TestHelper.Mock<IReweMobileApi>();
            mobileApiMock.Setup(x => x.SearchOffers(MarketId)).Returns(Task.FromResult(LoadJsonData<Envelope<OfferJso>>("2017-08-26_OfferSearch_193146.json")));

            var utilsMock = TestHelper.Mock<IReweUtils>();
            utilsMock.Setup(x => x.ParsePrice(It.IsAny<string>())).Returns(0.00M);
            utilsMock.Setup(x => x.Trim(It.IsAny<string>())).Returns((string x) => x);

            // Act
            var loader = new ReweOfferImporter(mobileApiMock.Object, utilsMock.Object, rawOfferMock.Object, dbContext, new ConsoleLogger<ReweOfferImporter>());
            var (result, offers) = await loader.ImportOffersFromApi(new Market { ExternalId = MarketId });

            // Assert
            Assert.IsInstanceOfType(result, typeof(SuccessResult));
            Assert.IsNotNull(dbContext.Products.FirstOrDefault());
            Assert.IsNotNull(dbContext.Offers.FirstOrDefault());

            mobileApiMock.VerifyAll();
            utilsMock.VerifyAll();
        }

        /// <summary>
        ///     Test product and offer creation
        /// </summary>
        [TestMethod]
        public async Task LoadOffers_ProductOfferCreation()
        {
            var dbContext = new OffersDbContext(new DbContextOptionsBuilder<OffersDbContext>().UseInMemoryDatabase("LoadOffers_ProductOfferCreation").Options,
                                                new ConsoleLogger<OffersDbContext>());

            var offer = new OfferJso
            {
                AdditionalFields = new Dictionary<string, string> { { "crossOutPrice", "329" } },
                Brand = "WEIMARER",
                CategoryIDs = Array.Empty<string>(),
                Id = "0038431_35_1931046",
                Name = "Orig. Thüringer Rostbratwurst",
                OfferDuration = new OfferDurationJso { From = DateTime.Now.GetPreviousWeekday(DayOfWeek.Saturday), Until = DateTime.Now.GetNextWeekday(DayOfWeek.Saturday) },
                Price = 2.79,
                ProductId = "0038431",
                QuantityAndUnit = "450-g-Packung"
            };

            var offers = new Envelope<OfferJso> { Items = new List<OfferJso> { offer }, Meta = new Dictionary<string, Newtonsoft.Json.Linq.JToken>() };

            var rawOfferMock = TestHelper.Mock<IRawOfferDataService>();
            rawOfferMock.Setup(x => x.Save(It.IsAny<string>(), It.IsAny<int>())).Returns(Task.FromResult(((Result)SuccessResult.Default, new RawOfferDataDto())));

            var mobileApiMock = TestHelper.Mock<IReweMobileApi>();
            mobileApiMock.Setup(x => x.SearchOffers(MarketId)).Returns(Task.FromResult(offers));

            var utilsMock = TestHelper.Mock<IReweUtils>();
            utilsMock.Setup(x => x.ParsePrice("329")).Returns(3.29M);
            utilsMock.Setup(x => x.Trim(It.IsAny<string>())).Returns((string x) => x);

            // Act
            var loader = new ReweOfferImporter(mobileApiMock.Object, utilsMock.Object, rawOfferMock.Object, dbContext, new ConsoleLogger<ReweOfferImporter>());
            var (result, importedOffers) = await loader.ImportOffersFromApi(new Market { ExternalId = MarketId });
            var savedOffer = dbContext.Offers.FirstOrDefault(o => o.ExternalId == offer.Id);
            var savedProduct = dbContext.Products.FirstOrDefault(p => p.ExternalId == offer.ProductId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(SuccessResult));
            Assert.IsNotNull(savedOffer);
            Assert.AreEqual((decimal)offer.Price, savedOffer.Price);
            Assert.AreEqual(offer.Id, savedOffer.ExternalId);
            Assert.AreEqual(DayOfWeek.Monday, savedOffer.From.DayOfWeek);
            Assert.AreEqual(DayOfWeek.Sunday, savedOffer.To.DayOfWeek);

            Assert.IsNotNull(savedProduct);
            Assert.AreEqual(3.29M, savedProduct.Price);
            Assert.AreEqual(offer.Brand, savedProduct.Brand);
            Assert.AreEqual(offer.ProductId, savedProduct.ExternalId);
            Assert.AreEqual(offer.Name, savedProduct.Name);
            Assert.AreEqual(offer.QuantityAndUnit, savedProduct.SizeInfo);
            Assert.AreEqual(1, savedProduct.PriceHistoryEntries.Count);
        }

        /// <summary>
        ///     Test price update of product
        /// </summary>
        [TestMethod]
        public async Task LoadOffers_ProductUpdate()
        {
            var dbContext = new OffersDbContext(new DbContextOptionsBuilder<OffersDbContext>().UseInMemoryDatabase("LoadOffers_ProductOfferUpdate").Options,
                                                new ConsoleLogger<OffersDbContext>());

            var offer = new OfferJso
            {
                AdditionalFields = new Dictionary<string, string> { { "crossOutPrice", "329" } },
                Brand = "WEIMARER",
                CategoryIDs = Array.Empty<string>(),
                Id = "0038431_35_1931046",
                Name = "Orig. Thüringer Rostbratwurst",
                OfferDuration = new OfferDurationJso { From = DateTime.Now.AddDays(-4), Until = DateTime.Now.AddDays(3) },
                Price = 2.79,
                ProductId = "0038431",
                QuantityAndUnit = "450-g-Packung"
            };

            var offers = new Envelope<OfferJso> { Items = new List<OfferJso> { offer }, Meta = new Dictionary<string, Newtonsoft.Json.Linq.JToken>() };

            var offer2 = JsonClone(offer);
            offer2.AdditionalFields["crossOutPrice"] = "339";
            var offers2 = new Envelope<OfferJso> { Items = new List<OfferJso> { offer2 }, Meta = new Dictionary<string, Newtonsoft.Json.Linq.JToken>() };

            var rawOfferMock = TestHelper.Mock<IRawOfferDataService>();
            rawOfferMock.Setup(x => x.Save(It.IsAny<string>(), It.IsAny<int>())).Returns(Task.FromResult(((Result)SuccessResult.Default, new RawOfferDataDto())));

            var mobileApiMock = TestHelper.Mock<IReweMobileApi>();
            mobileApiMock.SetupSequence(x => x.SearchOffers(MarketId)).Returns(Task.FromResult(offers)).Returns(Task.FromResult(offers2));

            var utilsMock = TestHelper.Mock<IReweUtils>();
            utilsMock.Setup(x => x.ParsePrice("329")).Returns(3.29M);
            utilsMock.Setup(x => x.ParsePrice("339")).Returns(3.39M);
            utilsMock.Setup(x => x.Trim(It.IsAny<string>())).Returns((string x) => x);

            // Act
            var loader = new ReweOfferImporter(mobileApiMock.Object, utilsMock.Object, rawOfferMock.Object, dbContext, new ConsoleLogger<ReweOfferImporter>());
            var (result, importedOffers) = await loader.ImportOffersFromApi(new Market { ExternalId = MarketId });
            var (result2, importedOffers2) = await loader.ImportOffersFromApi(new Market { ExternalId = MarketId });

            var savedProduct = dbContext.Products.FirstOrDefault(p => p.ExternalId == offer.ProductId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(SuccessResult));
            Assert.IsInstanceOfType(result2, typeof(SuccessResult));

            Assert.IsNotNull(savedProduct);
            Assert.AreEqual(3.39M, savedProduct.Price);
            Assert.AreEqual(offer.Brand, savedProduct.Brand);
            Assert.AreEqual(offer.ProductId, savedProduct.ExternalId);
            Assert.AreEqual(offer.Name, savedProduct.Name);
            Assert.AreEqual(offer.QuantityAndUnit, savedProduct.SizeInfo);
            Assert.AreEqual(2, savedProduct.PriceHistoryEntries.Count);
        }

        private T JsonClone<T>(T instance)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(instance));
        }

        private T LoadJsonData<T>(string name)
        {
            var content = TestHelper.ReadEmbeddedFile(GetType().Assembly, $"Data.{name}");
            return JsonConvert.DeserializeObject<T>(content);
        }
    }
}