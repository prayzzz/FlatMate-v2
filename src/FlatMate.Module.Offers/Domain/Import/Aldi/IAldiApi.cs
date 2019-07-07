using System.Threading.Tasks;
using RestEase;

namespace FlatMate.Module.Offers.Domain.Import.Aldi
{
    public interface IAldiApi
    {
        [Get("/aldiAngebot/")]
        Task<string> GetAreas();

        [Get("/aldiAngebot/teaser/{catrel}/{teaserXml}")]
        Task<string> GetOffers([Path] string catrel, [Path] string teaserXml);
    }
}