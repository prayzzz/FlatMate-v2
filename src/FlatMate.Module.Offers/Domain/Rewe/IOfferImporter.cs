using FlatMate.Module.Offers.Domain.Companies;
using FlatMate.Module.Offers.Domain.Markets;
using FlatMate.Module.Offers.Domain.Offers;
using FlatMate.Module.Offers.Domain.Products;
using FlatMate.Module.Offers.Domain.Raw;
using FlatMate.Module.Offers.Domain.Rewe.Jso;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using prayzzz.Common.Attributes;
using prayzzz.Common.Results;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FlatMate.Module.Offers.Domain.Rewe
{
    public interface IOfferImporter
    {
        Company Company { get; }

        Task<(Result, IEnumerable<Offer>)> ImportOffersFromRaw(Market market, string data);

        Task<(Result, IEnumerable<Offer>)> ImportOffersFromApi(Market market);
    }
}
