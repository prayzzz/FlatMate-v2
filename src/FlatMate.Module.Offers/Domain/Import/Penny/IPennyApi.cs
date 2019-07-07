using System.Threading.Tasks;
using FlatMate.Module.Offers.Domain.Import.Penny.Jso;
using RestEase;

namespace FlatMate.Module.Offers.Domain.Import.Penny
{
    public interface IPennyApi
    {
        [Get("/angebote_json.php")]
        Task<Envelope> GetOffers();
    }
}