using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;
using prayzzz.Common.Results;

namespace FlatMate.Module.Infrastructure.Images
{
    public interface IImageService
    {
        Task<Result> Delete(Guid guid);

        Task<(Result, ImageDto)> Get(Guid hash);

        Task<(Result, ImageDto)> Get(int id);

        Task<(Result, ImageDto)> Save(byte[] file, string contentType);
    }

    [Inject]
    public class ImageService : IImageService
    {
        /// <summary>
        ///     5 Megabyte
        /// </summary>
        private const long MaxImageSize = 5 * 1024 * 1024;

        private static readonly string[] SupportedContentTypes = { "image/jpg", "image/jpeg", "image/png" };
        private static readonly MemoryCacheEntryOptions CacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(24));

        private readonly IMemoryCache _cache;
        private readonly InfrastructureDbContext _dbContext;
        private readonly ILogger<ImageService> _logger;
        private readonly IMapper _mapper;

        public ImageService(IMemoryCache cache, InfrastructureDbContext dbContext, IMapper mapper, ILogger<ImageService> logger)
        {
            _cache = cache;
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        public Task<Result> Delete(Guid guid)
        {
            _dbContext.Images.RemoveRange(_dbContext.Images.Where(i => i.Guid == guid));
            return Task.FromResult(Result.Success);
        }

        public async Task<(Result, ImageDto)> Get(int id)
        {
            var image = await _dbContext.Images.FindAsync(id);

            if (image == null)
            {
                return (new Result(ErrorType.NotFound, "Image not found"), null);
            }

            return (Result.Success, _mapper.Map<ImageDto>(image));
        }

        public async Task<(Result, ImageDto)> Get(Guid hash)
        {
            if (!_cache.TryGetValue(hash, out var image))
            {
                image = await _dbContext.Images.FirstOrDefaultAsync(x => x.Guid == hash);
                if (image == null)
                {
                    return (new Result(ErrorType.NotFound, "Image not found"), null);
                }

                _cache.Set(hash, image, CacheEntryOptions);
            }

            return (Result.Success, _mapper.Map<ImageDto>(image));
        }

        public async Task<(Result, ImageDto)> Save(byte[] file, string contentType)
        {
            if (file.LongLength > MaxImageSize)
            {
                return (new Result(ErrorType.ValidationError, "File to large"), null);
            }

            if (!SupportedContentTypes.Contains(contentType, StringComparer.CurrentCultureIgnoreCase))
            {
                return (new Result(ErrorType.ValidationError, "Unsupported media type"), null);
            }

            var image = new Image { File = file, Guid = Guid.NewGuid(), ContentType = contentType.ToLower() };
            await _dbContext.AddAsync(image);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(0, e, "Error while saving changes");
                return (new Result(ErrorType.InternalError, "Datenbankfehler"), null);
            }

            return (Result.Success, _mapper.Map<ImageDto>(image));
        }
    }
}