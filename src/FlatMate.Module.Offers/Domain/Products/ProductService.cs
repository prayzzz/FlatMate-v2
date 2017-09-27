using FlatMate.Module.Offers.Domain.Products;
using Microsoft.EntityFrameworkCore;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;
using prayzzz.Common.Results;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlatMate.Module.Offers.Domain
{
    public interface IProductService
    {
        Task<Result> AddProductFavorite(int userId, int productId);

        Task<List<ProductDto>> GetFavoriteProducts(int userId, int marketId);

        Task<(Result, ProductDto)> GetProduct(int id);

        Task<List<ProductCategoryDto>> GetProductCategories();

        Task<List<OfferDto>> GetProductOffers(int id);

        Task<List<PriceHistoryDto>> GetProductPriceHistory(int productId);

        Task<List<ProductDto>> GetProducts(int marketId);
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

        public async Task<Result> AddProductFavorite(int userId, int productId)
        {
            if (await _dbContext.ProductFavorites.AnyAsync(f => f.ProductId == productId && f.UserId == userId))
            {
                return SuccessResult.Default;
            }

            if (!await _dbContext.Products.AnyAsync(p => p.Id == productId))
            {
                return new ErrorResult(ErrorType.ValidationError, "Product not found");
            }

            var favorite = new ProductFavorite { ProductId = productId, UserId = userId };

            _dbContext.ProductFavorites.Add(favorite);
            return await _dbContext.SaveChangesAsync();
        }

        public Task<List<ProductDto>> GetFavoriteProducts(int userId, int marketId)
        {
            return (from p in _dbContext.Products.Include(p => p.Market)
                    join f in _dbContext.ProductFavorites on p.Id equals f.Id
                    where f.UserId == userId && p.MarketId == marketId
                    select _mapper.Map<ProductDto>(p)).ToListAsync();
        }

        public async Task<(Result, ProductDto)> GetProduct(int id)
        {
            var product = await _dbContext.Products.FindAsync(id);
            if (product == null)
            {
                return (new ErrorResult(ErrorType.NotFound, "Product not found"), null);
            }

            return (SuccessResult.Default, _mapper.Map<ProductDto>(product));
        }

        public Task<List<ProductCategoryDto>> GetProductCategories()
        {
            return (from p in _dbContext.ProductCategories
                    select _mapper.Map<ProductCategoryDto>(p)).ToListAsync();
        }

        public Task<List<OfferDto>> GetProductOffers(int id)
        {
            return (from o in _dbContext.Offers.Include(o => o.Market)
                    where o.ProductId == id
                    select _mapper.Map<OfferDto>(o)).ToListAsync();
        }

        public Task<List<PriceHistoryDto>> GetProductPriceHistory(int productId)
        {
            return (from ph in _dbContext.PriceHistoryEntries
                    where ph.ProductId == productId
                    select _mapper.Map<PriceHistoryDto>(ph)).ToListAsync();
        }

        public Task<List<ProductDto>> GetProducts(int marketId)
        {
            return (from p in _dbContext.Products
                    where p.MarketId == marketId
                    select _mapper.Map<ProductDto>(p)).ToListAsync();
        }
    }
}
