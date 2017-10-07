using RestEase;
using System.Threading.Tasks;

namespace FlatMate.Module.Offers.Domain.Adapter.Aldi
{
    public interface IAldiApi
    {
        [Get("/aldiAngebot/")]
        Task<string> GetAreas();

        [Get("/aldiAngebot/teaser/{catrel}/{teaserXml}")]
        Task<string> GetOffers([Path]string catrel, [Path]string teaserXml);
    }
}
