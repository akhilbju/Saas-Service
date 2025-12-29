using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Saas_Auth_Service.Controller
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ProjectController
    {
        /// <summary>
        /// Jwt Settings Service
        /// </summary>
        private readonly IJwtSettings _jwtSettings;

        /// <summary>
        /// Password Hasher Service
        /// </summary>
        private readonly IPasswordHasherService _passwordHasherService;

        /// <summary>
        /// Database Context
        /// </summary>
        private readonly AppDbContext _dbContext;

        /// <summary>
        /// Http Context Accessor
        /// </summary>
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Cache Service
        /// </summary>
        private readonly ICacheService _cacheService;

        public ProjectController(IJwtSettings jwtSettings, IPasswordHasherService passwordHasherService, AppDbContext dbContext, IHttpContextAccessor httpContextAccessor, ICacheService cacheService)
        {
            _jwtSettings = jwtSettings;
            _passwordHasherService = passwordHasherService;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _cacheService = cacheService;
        }

        #region  public methods
        [Authorize(Roles = $"{UserType.MANAGER},{UserType.ADMIN}")]
        [HttpPost]
        public Response CreateProject(CreateProjectRequest request)
        {
            var response = new Response();
            var project = new Project
            {
                Name = request.Name,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow,
                CreatedById = Convert.ToInt32(_httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)),
                StartDate = System.TimeZoneInfo.ConvertTimeToUtc(request.StartDate, System.TimeZoneInfo.FindSystemTimeZoneById(request.TimeZone)),
                EndDate = System.TimeZoneInfo.ConvertTimeToUtc(request.EndDate, System.TimeZoneInfo.FindSystemTimeZoneById(request.TimeZone)),
                TimeZone = request.TimeZone,
            };
            foreach (var memberId in request.TeamMemberIds)
            {
                var user = _dbContext.Users.Find(memberId);
                if (user != null)
                {
                    project.TeamMembers.Add(user);
                }
            }
            _dbContext.Users.Find(project.CreatedById)!.CreatedProjects.Add(project);
            _dbContext.Projects.Add(project);
            _dbContext.SaveChanges();
            response.IsSuccess = true;
            response.Message = SuccessMessages.ProjectCreationSuccess;
            return response;
        }

        [HttpPost]
        public async Task<GetAllProjects> GetProjectsAsync(GetProjectRequest request)
        {
            var keyRaw = JsonSerializer.Serialize(request);
            var cacheKey = "projects:" + Convert.ToHexString(
                SHA256.HashData(Encoding.UTF8.GetBytes(keyRaw)));

            return await _cacheService.GetOrCreateAsync(cacheKey, async () =>
            {
                var response = new GetAllProjects();

                IQueryable<Project> query = _dbContext.Projects.AsQueryable();

                if (!string.IsNullOrWhiteSpace(request.ProjectName))
                {
                    query = query.Where(x =>
                        EF.Functions.Like(x.Name, $"%{request.ProjectName}%"));
                }

                int pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
                int skip = (pageNumber - 1) * request.RowsPerPage;

                var projects = await query
                    .Include(p => p.TeamMembers)
                    .Skip(skip)
                    .Take(request.RowsPerPage)
                    .ToListAsync();

                response.Count = projects.Count;

                response.Projects = projects.Select(project => new ProjectResponse
                {
                    Id = project.Id,
                    Name = project.Name,
                    Description = project.Description,
                    TeamMembers = project.TeamMembers.Select(tm => new UserResponse
                    {
                        Id = tm.Id,
                        Username = tm.Username,
                        Email = tm.Email,
                    }).ToList()
                }).ToList();

                return response;

            }, expirationInMinutes: 3);
        }
        #endregion

        #region private methods
        #endregion
    }
}