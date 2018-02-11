using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlatMate.Module.Account.Shared;
using FlatMate.Module.Account.Shared.Interfaces;
using FlatMate.Module.Common.Api;
using FlatMate.Module.Common.Domain;
using FlatMate.Module.Offers.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;
using prayzzz.Common.Results;

namespace FlatMate.Module.Offers.Domain
{
    public interface IProductService
    {
        Task<Result> AddProductFavorite(int productId);

        Task<Result> DeleteProductFavorite(int productId);

        Task<List<ProductInfoDto>> GetDuplicateProducts();

        Task<List<int>> GetFavoriteProductIds(Company company);

        Task<List<ProductDto>> GetFavoriteProducts(Company company);

        Task<(Result, ProductDto)> GetProduct(int id);

        Task<List<ProductCategoryDto>> GetProductCategories();

        Task<List<OfferDto>> GetProductOffers(int id);

        Task<List<PriceHistoryDto>> GetProductPriceHistory(int productId);

        Task<Result> MergeProducts(int productId, int otherProductId);

        Task Migrate();

        Task<PartialList<ProductDto>> SearchProducts(Company company, string searchTerm, PartialListParameter parameter);

        Task<PartialList<ProductDto>> SearchFavoriteProducts(Company company, string searchTerm, PartialListParameter parameter);
    }

    [Inject]
    public partial class ProductService : IProductService
    {
        private readonly IAuthenticationContext _authenticationContext;
        private readonly OffersDbContext _dbContext;
        private readonly ILogger<ProductService> _logger;
        private readonly IMapper _mapper;

        public ProductService(OffersDbContext dbContext,
                              IAuthenticationContext authenticationContext,
                              ILogger<ProductService> logger,
                              IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _authenticationContext = authenticationContext;
            _logger = logger;
        }

        private CurrentUser CurrentUser => _authenticationContext.CurrentUser;

        public async Task<Result> AddProductFavorite(int productId)
        {
            if (await _dbContext.ProductFavorites.AnyAsync(f => f.ProductId == productId && f.UserId == CurrentUser.Id))
            {
                return Result.Success;
            }

            if (!await _dbContext.Products.AnyAsync(p => p.Id == productId))
            {
                return new Result(ErrorType.ValidationError, "Product not found");
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

            return Result.Success;
        }

        public Task<List<int>> GetFavoriteProductIds(Company company)
        {
            return (from p in _dbContext.Products
                    join f in _dbContext.ProductFavorites on p.Id equals f.ProductId
                    where f.UserId == CurrentUser.Id && p.CompanyId == (int) company
                    select p.Id).ToListAsync();
        }

        public Task<List<ProductDto>> GetFavoriteProducts(Company company)
        {
            return (from p in _dbContext.Products
                    join f in _dbContext.ProductFavorites on p.Id equals f.ProductId
                    where f.UserId == CurrentUser.Id && p.CompanyId == (int) company
                    select _mapper.Map<ProductDto>(p)).AsNoTracking().ToListAsync();
        }

        public async Task<(Result, ProductDto)> GetProduct(int id)
        {
            var product = await _dbContext.Products.Include(x => x.CompanyData).FirstOrDefaultAsync(x => x.Id == id);
            if (product == null)
            {
                return (new Result(ErrorType.NotFound, "Product not found"), null);
            }

            return (Result.Success, _mapper.Map<ProductDto>(product));
        }

        public Task<List<ProductCategoryDto>> GetProductCategories()
        {
            return (from p in _dbContext.ProductCategories
                    select _mapper.Map<ProductCategoryDto>(p)).AsNoTracking().ToListAsync();
        }

        public Task<List<OfferDto>> GetProductOffers(int id)
        {
            return (from o in _dbContext.Offers.Include(o => o.Market)
                    where o.ProductId == id
                    select _mapper.Map<OfferDto>(o)).AsNoTracking().ToListAsync();
        }

        public Task<List<PriceHistoryDto>> GetProductPriceHistory(int productId)
        {
            return (from ph in _dbContext.PriceHistoryEntries.Include(ph => ph.Product)
                    where ph.ProductId == productId
                    select _mapper.Map<PriceHistoryDto>(ph)).AsNoTracking().ToListAsync();
        }

        public Task<PartialList<ProductDto>> SearchProducts(Company company, string searchTerm, PartialListParameter parameter)
        {
            return SearchProducts(company, searchTerm, false, parameter);
        }

        public Task<PartialList<ProductDto>> SearchFavoriteProducts(Company company, string searchTerm, PartialListParameter parameter)
        {
            return SearchProducts(company, searchTerm, true, parameter);
        }

        private async Task<PartialList<ProductDto>> SearchProducts(Company company, string searchTerm, bool isFavorite, PartialListParameter parameter)
        {
            IQueryable<Product> productsQuery = _dbContext.Products;

            if (isFavorite)
            {
                productsQuery = from p in productsQuery
                                join f in _dbContext.ProductFavorites on p.Id equals f.ProductId
                                where f.UserId == CurrentUser.Id
                                select p;
            }

            if (company != Company.None)
            {
                productsQuery = productsQuery.Where(p => p.CompanyId == (int) company);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                productsQuery = productsQuery.Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm) || p.Brand.Contains(searchTerm));
            }

            var totalCountTask = productsQuery.CountAsync();
            var productsQueryTask = productsQuery.OrderByDescending(p => p.Id)
                                                 .Skip(parameter.Offset)
                                                 .Take(parameter.Limit)
                                                 .AsNoTracking()
                                                 .ToListAsync();

            var totalCount = await totalCountTask;
            var products = await productsQueryTask;

            return new PartialList<ProductDto>(products.Select(_mapper.Map<ProductDto>), parameter, totalCount);
        }
    }
}