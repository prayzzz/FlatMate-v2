using System.Threading.Tasks;
using FlatMate.Module.Offers.Domain.Adapter.Penny.Jso;
using RestEase;

namespace FlatMate.Module.Offers.Domain.Adapter.Penny
{
    public interface IPennyApi
    {
        [Get("/angebote_json.php")]
        Task<Envelope> GetOffers();
    }
}