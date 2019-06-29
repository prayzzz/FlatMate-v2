using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlatMate.Module.Offers.Domain.Adapter.Penny;
using FlatMate.Module.Offers.Domain.Adapter.Penny.Jso;
using FlatMate.Module.Offers.Domain.Markets;
using FlatMate.Module.Offers.Domain.Raw;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using prayzzz.Common.Results;
using prayzzz.Common.Unit;

namespace FlatMate.Module.Offers.Test.Domain.Adapter.Penny
{
    [TestClass]
    public class PennyOfferImporterTest
    {
        /// <summary>
        /// Sometimes there're duplicates in the offers returned by the api.
        /// The importer should remove them.
        /// </summary>
        [TestMethod]
        public async Task ImportOffersFromApiDuplicate()
        {
            var dbContext = new OffersDbContext(new DbContextOptionsBuilder<OffersDbContext>().UseInMemoryDatabase("ImportOffersFromApiDuplicate").Options,
                                                new ConsoleLogger<OffersDbContext>());

            var offerEnvelop = new Envelope
            {
                Offers = new List<OfferJso>
                {
                    new OfferJso { Titel = "Titel", Beschreibung = "Offer1", Menge = "Size" },
                    new OfferJso { Titel = "Titel", Beschreibung = "Offer2", Menge = "Size" }
                }
            };

            var apiMock = TestHelper.Mock<IPennyApi>();
            apiMock.Setup(x => x.GetOffers()).Returns(Task.FromResult(offerEnvelop));

            var rawOfferMock = TestHelper.Mock<IRawOfferDataService>();
            rawOfferMock.Setup(x => x.Save(It.IsAny<string>(), 0)).Returns(Task.FromResult<(Result, RawOfferDataDto)>((Result.Success, null)));

            var utilsMocks = TestHelper.Mock<IPennyUtils>();
            utilsMocks.Setup(x => x.StripHtml(It.IsAny<string>())).Returns((string x) => x);
            utilsMocks.Setup(x => x.ParsePrice(It.IsAny<string>())).Returns((string x) => 0);
            utilsMocks.Setup(x => x.Trim(It.IsAny<string>())).Returns((string x) => x);

            var logger = new ConsoleLogger<PennyOfferImporter>();

            // Act
            await new PennyOfferImporter(dbContext, apiMock.Object, rawOfferMock.Object, utilsMocks.Object, logger).ImportOffersFromApi(new Market());

            // Assert
            Assert.AreEqual(1, dbContext.Offers.Count());

            apiMock.VerifyAll();
            rawOfferMock.VerifyAll();
            utilsMocks.VerifyAll();
        }
    }
}