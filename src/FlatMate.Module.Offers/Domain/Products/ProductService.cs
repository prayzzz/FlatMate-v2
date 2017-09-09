using FlatMate.Module.Offers.Domain.Offers;
using Microsoft.EntityFrameworkCore;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;
using prayzzz.Common.Results;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlatMate.Module.Offers.Domain.Products
{
    public interface IProductService
    {
        Task<(Result, ProductDto)> GetProduct(int id);

        Task<IEnumerable<ProductCategoryDto>> GetProductCategories();

        Task<IEnumerable<PriceHistoryDto>> GetProductPriceHistory(int productId);

        Task<IEnumerable<ProductDto>> GetProducts();

        Task<IEnumerable<OfferDto>> GetProductOffers(int id);
    }

    [Inject]
    public class ProductService : IProductService
    {
        private readonly OffersDbContext _dbContext;

        private readonly IMapper _mapper;

        public ProductService(OffersDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<(Result, ProductDto)> GetProduct(int id)
        {
            var product = await _dbContext.Product.FindAsync(id);
            if (product == null)
            {
                return (new ErrorResult(ErrorType.NotFound, "Product not found"), null);
            }

            return (SuccessResult.Default, _mapper.Map<ProductDto>(product));
        }

        public async Task<IEnumerable<ProductCategoryDto>> GetProductCategories()
        {
            return (await _dbContext.ProductCategories.ToListAsync()).Select(_mapper.Map<ProductCategoryDto>);
        }

        public async Task<IEnumerable<OfferDto>> GetProductOffers(int id)
        {
            return (await _dbContext.Offers.Where(o => o.ProductId == id).ToListAsync()).Select(_mapper.Map<OfferDto>);
        }

        public async Task<IEnumerable<PriceHistoryDto>> GetProductPriceHistory(int productId)
        {
            return (await _dbContext.PriceHistory.Where(ph => ph.ProductId == productId).ToListAsync()).Select(_mapper.Map<PriceHistoryDto>);
        }

        public async Task<IEnumerable<ProductDto>> GetProducts()
        {
            return (await _dbContext.Product.ToListAsync()).Select(_mapper.Map<ProductDto>);
        }
    }
}
