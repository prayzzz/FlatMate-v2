using FlatMate.Module.Common;
using FlatMate.Module.Offers.Domain.Adapter.Aldi;
using FlatMate.Module.Offers.Domain.Adapter.Aldi.Jso;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestEase;
using System;
using System.Threading.Tasks;

namespace FlatMate.Module.Offers.Test.Aldi
{
    [TestClass]
    public class AldiApiTest
    {
        [TestMethod]
        public async Task TestApi()
        {
            var api = RestClient.For<IAldiApi>("http://ws.aldi-nord.de/");

            var areas = await api.GetAreas();
            var asd = XmlConvert.Deserialize<Data>(areas);

            Console.Write("");
        }
    }
}
