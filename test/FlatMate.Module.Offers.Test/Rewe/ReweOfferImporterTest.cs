﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlatMate.Module.Offers.Domain.Markets;
using FlatMate.Module.Offers.Domain.Rewe;
using FlatMate.Module.Offers.Domain.Rewe.Jso;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using prayzzz.Common.Results;
using prayzzz.Common.Unit;

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

            var mobileApiMock = TestHelper.Mock<IReweMobileApi>();
            mobileApiMock.Setup(x => x.SearchOffers(MarketId)).Returns(Task.FromResult(LoadJsonData<Envelope<OfferJso>>("2017-08-26_OfferSearch_193146.json")));

            var utilsMock = TestHelper.Mock<IReweUtils>();
            utilsMock.Setup(x => x.ParsePrice(It.IsAny<string>())).Returns(0.00M);

            // Act
            var loader = new ReweOfferImporter(mobileApiMock.Object, utilsMock.Object, dbContext, new ConsoleLogger<ReweOfferImporter>());
            var result = await loader.ImportOffersFromApi(new Market { ExternalId = MarketId });

            // Assert
            Assert.IsInstanceOfType(result, typeof(SuccessResult));
            Assert.IsNotNull(dbContext.Product.FirstOrDefault());
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
                Id = "0038431_35_1931046",
                Name = "Orig. Thüringer Rostbratwurst",
                OfferDuration = new OfferDurationJso { From = DateTime.Now.AddDays(-4), Until = DateTime.Now.AddDays(3) },
                Price = 2.79,
                ProductId = "0038431",
                QuantityAndUnit = "450-g-Packung"
            };

            var offers = new Envelope<OfferJso> { Items = new List<OfferJso> { offer } };

            var mobileApiMock = TestHelper.Mock<IReweMobileApi>();
            mobileApiMock.Setup(x => x.SearchOffers(MarketId)).Returns(Task.FromResult(offers));

            var utilsMock = TestHelper.Mock<IReweUtils>();
            utilsMock.Setup(x => x.ParsePrice("329")).Returns(3.29M);

            // Act
            var loader = new ReweOfferImporter(mobileApiMock.Object, utilsMock.Object, dbContext, new ConsoleLogger<ReweOfferImporter>());
            var result = await loader.ImportOffersFromApi(new Market { ExternalId = MarketId });
            var savedOffer = dbContext.Offers.FirstOrDefault(o => o.ExternalId == offer.Id);
            var savedProduct = dbContext.Product.FirstOrDefault(p => p.ExternalId == offer.ProductId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(SuccessResult));
            Assert.IsNotNull(savedOffer);
            Assert.AreEqual(offer.Price, savedOffer.Price);
            Assert.AreEqual(offer.Id, savedOffer.ExternalId);
            Assert.AreEqual(offer.OfferDuration.From, savedOffer.From);
            Assert.AreEqual(offer.OfferDuration.Until, savedOffer.To);

            Assert.IsNotNull(savedProduct);
            Assert.AreEqual(3.29, savedProduct.Price);
            Assert.AreEqual(offer.Brand, savedProduct.Brand);
            Assert.AreEqual(offer.ProductId, savedProduct.ExternalId);
            Assert.AreEqual(offer.Name, savedProduct.Name);
            Assert.AreEqual(offer.QuantityAndUnit, savedProduct.SizeInfo);
            Assert.AreEqual(1, savedProduct.PriceHistory.Count());
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
                Id = "0038431_35_1931046",
                Name = "Orig. Thüringer Rostbratwurst",
                OfferDuration = new OfferDurationJso { From = DateTime.Now.AddDays(-4), Until = DateTime.Now.AddDays(3) },
                Price = 2.79,
                ProductId = "0038431",
                QuantityAndUnit = "450-g-Packung"
            };

            var offers = new Envelope<OfferJso> { Items = new List<OfferJso> { offer } };

            var offer2 = JsonClone(offer);
            offer2.AdditionalFields["crossOutPrice"] = "339";
            var offers2 = new Envelope<OfferJso> { Items = new List<OfferJso> { offer2 } };

            var mobileApiMock = TestHelper.Mock<IReweMobileApi>();
            mobileApiMock.SetupSequence(x => x.SearchOffers(MarketId)).Returns(Task.FromResult(offers)).Returns(Task.FromResult(offers2));

            var utilsMock = TestHelper.Mock<IReweUtils>();
            utilsMock.Setup(x => x.ParsePrice("329")).Returns(3.29M);
            utilsMock.Setup(x => x.ParsePrice("339")).Returns(3.39M);

            // Act
            var loader = new ReweOfferImporter(mobileApiMock.Object, utilsMock.Object, dbContext, new ConsoleLogger<ReweOfferImporter>());
            var result = await loader.ImportOffersFromApi(new Market { ExternalId = MarketId });
            var result2 = await loader.ImportOffersFromApi(new Market { ExternalId = MarketId });

            var savedProduct = dbContext.Product.FirstOrDefault(p => p.ExternalId == offer.ProductId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(SuccessResult));
            Assert.IsInstanceOfType(result2, typeof(SuccessResult));

            Assert.IsNotNull(savedProduct);
            Assert.AreEqual(3.39, savedProduct.Price);
            Assert.AreEqual(offer.Brand, savedProduct.Brand);
            Assert.AreEqual(offer.ProductId, savedProduct.ExternalId);
            Assert.AreEqual(offer.Name, savedProduct.Name);
            Assert.AreEqual(offer.QuantityAndUnit, savedProduct.SizeInfo);
            Assert.AreEqual(2, savedProduct.PriceHistory.Count());
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