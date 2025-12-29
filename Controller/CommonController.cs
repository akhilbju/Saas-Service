using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]/[action]")]
[ApiController]
public class CommonController : ControllerBase
{
    /// <summary>
    /// Database Context
    /// </summary>
    private readonly AppDbContext _dbContext;

    private readonly ICacheService _cacheService;

    public CommonController(AppDbContext dbContext, ICacheService cacheService)
    {
        _dbContext = dbContext;
        _cacheService = cacheService;
    }

    [HttpGet]
    public async Task<List<GetUsers>> GetUsers()
    {
        var cacheKey = "GetUsers";
        return await _cacheService.GetOrCreateAsync(cacheKey, async () =>
        {
            return _dbContext.Users
                .Select(u => new GetUsers
                {
                    Id = u.Id,
                    Name = u.Username
                })
                .ToList();
        }, expirationInMinutes: 10 );
    }
}