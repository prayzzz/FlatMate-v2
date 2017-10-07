using RestEase;
using System.Threading.Tasks;

namespace FlatMate.Module.Offers.Domain.Adapter.Penny
{
    public interface IPennyApi
    {
        [Get("/angebote_json.php")]
        Task<Envelope> GetOffers();
    }
}
