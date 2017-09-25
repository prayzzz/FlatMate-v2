using prayzzz.Common.Results;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlatMate.Module.Offers.Domain
{
    public interface IOfferImporter
    {
        Company Company { get; }

        Task<(Result, IEnumerable<Offer>)> ImportOffersFromRaw(Market market, string data);

        Task<(Result, IEnumerable<Offer>)> ImportOffersFromApi(Market market);
    }
}