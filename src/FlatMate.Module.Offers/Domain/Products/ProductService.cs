using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlatMate.Module.Account.Shared;
using FlatMate.Module.Account.Shared.Interfaces;
using FlatMate.Module.Offers.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;
using prayzzz.Common.Results;

namespace FlatMate.Module.Offers.Domain
{
    public interface IProductService
    {
        Task<Result> AddProductFavorite(int productId);

        Task<Result> DeleteProductFavorite(int productId);

        Task<List<ProductDto>> GetDuplicateProducts();

        Task<List<int>> GetFavoriteProductIds(int marketId);

        Task<List<ProductDto>> GetFavoriteProducts(int marketId);

        Task<(Result, ProductDto)> GetProduct(int id);

        Task<List<ProductCategoryDto>> GetProductCategories();

        Task<List<OfferDto>> GetProductOffers(int id);

        Task<List<PriceHistoryDto>> GetProductPriceHistory(int productId);

        Task<List<ProductDto>> GetProducts(int marketId);

        Task<Result> MergeProducts(int productId, int otherProductId);
    }

    [Inject]
    public partial class ProductService : IProductService
    {
        private const string CachePrefix = "Offers.Products";
        private static readonly MemoryCacheEntryOptions CacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(2));

        private readonly IAuthenticationContext _authenticationContext;
        private readonly IMemoryCache _cache;
        private readonly OffersDbContext _dbContext;
        private readonly IMapper _mapper;

        public ProductService(OffersDbContext dbContext,
                              IMemoryCache cache,
                              IAuthenticationContext authenticationContext,
                              IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _authenticationContext = authenticationContext;
            _cache = cache;
        }

        private CurrentUser CurrentUser => _authenticationContext.CurrentUser;

        public async Task<Result> AddProductFavorite(int productId)
        {
            if (await _dbContext.ProductFavorites.AnyAsync(f => f.ProductId == productId && f.UserId == CurrentUser.Id))
            {
                return SuccessResult.Default;
            }

            if (!await _dbContext.Products.AnyAsync(p => p.Id == productId))
            {
                return new ErrorResult(ErrorType.ValidationError, "Product not found");
            }

            var favorite = new ProductFavorite { ProductId = productId, UserId = CurrentUser.Id };

            _dbContext.ProductFavorites.Add(favorite);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<Result> DeleteProductFavorite(int productId)
        {
            var favorite = await _dbContext.ProductFavorites.FirstOrDefaultAsync(pf => pf.ProductId == productId && pf.UserId == CurrentUser.Id);
            if (favorite != null)
            {
                _dbContext.Remove(favorite);
                return await _dbContext.SaveChangesAsync();
            }

            return SuccessResult.Default;
        }

        public Task<List<int>> GetFavoriteProductIds(int marketId)
        {
            return (from p in _dbContext.Products
                    join f in _dbContext.ProductFavorites on p.Id equals f.ProductId
                    where f.UserId == CurrentUser.Id && p.MarketId == marketId
                    select p.Id).ToListAsync();
        }

        public Task<List<ProductDto>> GetFavoriteProducts(int marketId)
        {
            return (from p in _dbContext.Products
                    join f in _dbContext.ProductFavorites on p.Id equals f.ProductId
                    where f.UserId == CurrentUser.Id && p.MarketId == marketId
                    select _mapper.Map<ProductDto>(p)).ToListAsync();
        }

        public async Task<(Result, ProductDto)> GetProduct(int id)
        {
            var product = await _dbContext.Products.Include(x => x.Market).FirstOrDefaultAsync(x => x.Id == id);
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
            return (from ph in _dbContext.PriceHistoryEntries.Include(ph => ph.Product)
                    where ph.ProductId == productId
                    select _mapper.Map<PriceHistoryDto>(ph)).ToListAsync();
        }

        public async Task<List<ProductDto>> GetProducts(int marketId)
        {
            var cacheKey = $"{CachePrefix}_market-{marketId}";
            if (!_cache.TryGetValue(cacheKey, out List<Product> products))
            {
                products = await (from p in _dbContext.Products
                                  where p.MarketId == marketId
                                  select p).ToListAsync();

                if (products.Count > 0)
                {
                    _cache.Set(cacheKey, products, CacheEntryOptions);
                }
            }

            return products.Select(_mapper.Map<ProductDto>).ToList();
        }
    }
}