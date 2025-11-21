using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using LibraryManagement.Domain.Interfaces;

namespace LibraryManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CachingDemoController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMemoryCache _cache;
    private const string BooksCacheKey = "AllBooks";
    private const string MembersCacheKey = "AllMembers";

    public CachingDemoController(IUnitOfWork unitOfWork, IMemoryCache cache)
    {
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    [HttpGet("info")]
    public IActionResult GetInfo()
    {
        return Ok(new
        {
            title = "Memory Caching (ПР8)",
            description = "Кешування даних в памʼяті для покращення продуктивності",
            cacheKeys = new[] { BooksCacheKey, MembersCacheKey },
            endpoints = new
            {
                info = "GET /api/cachingdemo/info",
                books = "GET /api/cachingdemo/books",
                booksNoCache = "GET /api/cachingdemo/books/no-cache",
                members = "GET /api/cachingdemo/members",
                clearCache = "DELETE /api/cachingdemo/cache"
            }
        });
    }

    [HttpGet("books")]
    public async Task<IActionResult> GetBooksWithCache()
    {
        var startTime = DateTime.UtcNow;

        if (!_cache.TryGetValue(BooksCacheKey, out object? cachedBooks))
        {
            var books = await _unitOfWork.Books.GetAllAsync();
            
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5))
                .SetSlidingExpiration(TimeSpan.FromMinutes(2));
            
            _cache.Set(BooksCacheKey, books, cacheOptions);
            
            var loadTime = (DateTime.UtcNow - startTime).TotalMilliseconds;
            
            return Ok(new
            {
                source = "Database",
                loadTimeMs = loadTime,
                count = books.Count(),
                data = books,
                cache = new
                {
                    absoluteExpiration = "5 minutes",
                    slidingExpiration = "2 minutes"
                }
            });
        }

        var cachedLoadTime = (DateTime.UtcNow - startTime).TotalMilliseconds;
        
        return Ok(new
        {
            source = "Cache",
            loadTimeMs = cachedLoadTime,
            data = cachedBooks
        });
    }

    [HttpGet("books/no-cache")]
    public async Task<IActionResult> GetBooksNoCache()
    {
        var startTime = DateTime.UtcNow;
        var books = await _unitOfWork.Books.GetAllAsync();
        var loadTime = (DateTime.UtcNow - startTime).TotalMilliseconds;

        return Ok(new
        {
            source = "Database (без кешу)",
            loadTimeMs = loadTime,
            count = books.Count(),
            data = books
        });
    }

    [HttpGet("members")]
    public async Task<IActionResult> GetMembersWithCache()
    {
        var startTime = DateTime.UtcNow;

        if (!_cache.TryGetValue(MembersCacheKey, out object? cachedMembers))
        {
            var members = await _unitOfWork.Members.GetAllAsync();
            
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(10))
                .SetPriority(CacheItemPriority.High);
            
            _cache.Set(MembersCacheKey, members, cacheOptions);
            
            var loadTime = (DateTime.UtcNow - startTime).TotalMilliseconds;
            
            return Ok(new
            {
                source = "Database",
                loadTimeMs = loadTime,
                count = members.Count(),
                data = members,
                cache = new
                {
                    absoluteExpiration = "10 minutes",
                    priority = "High"
                }
            });
        }

        var cachedLoadTime = (DateTime.UtcNow - startTime).TotalMilliseconds;
        
        return Ok(new
        {
            source = "Cache",
            loadTimeMs = cachedLoadTime,
            data = cachedMembers
        });
    }

    [HttpDelete("cache")]
    public IActionResult ClearCache()
    {
        _cache.Remove(BooksCacheKey);
        _cache.Remove(MembersCacheKey);

        return Ok(new
        {
            message = "Кеш очищено",
            clearedKeys = new[] { BooksCacheKey, MembersCacheKey }
        });
    }
}
