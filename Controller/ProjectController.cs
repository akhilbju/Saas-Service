using System.Security.Claims;
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

        public ProjectController(IJwtSettings jwtSettings, IPasswordHasherService passwordHasherService, AppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _jwtSettings = jwtSettings;
            _passwordHasherService = passwordHasherService;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
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
                StartDate = request.StartDate,
                EndDate = request.EndDate,
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
        public GetAllProjects GetProjects(GetProjectRequest request)
        {
            var response = new GetAllProjects();
            var userId = Convert.ToInt32(_httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier));
            IQueryable<Project> query = _dbContext.Projects.AsQueryable();
            if (!string.IsNullOrEmpty(request.ProjectName))
            {
                query = query.Where(x => EF.Functions.Like(x.Name, $"%{request.ProjectName}%"));
            }
            int pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
            int skip = (pageNumber - 1) * request.RowsPerPage;
            var projects = query.Skip(skip).Take(request.RowsPerPage).ToList();
            response.Count = projects.Count;
            foreach (var project in projects)
            {
                var projectDto = new ProjectResponse
                {
                    Id = project.Id,
                    Name = project.Name,
                    Description = project.Description,
                    TeamMembers = project.TeamMembers.Select(tm => new UserResponse
                    {
                        Id = tm.Id,
                        Username = tm.Username,
                        Email = tm.Email,
                    }).ToList(),
                };
                response.Projects.Add(projectDto);
            }
            return response;
        }
        #endregion

        #region private methods
        #endregion
    }
}