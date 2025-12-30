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

        /// <summary>
        /// Create Project
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
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
            _cacheService.Remove("GetProjectsAsync");
            response.IsSuccess = true;
            response.Message = SuccessMessages.ProjectCreationSuccess;
            return response;
        }

        /// <summary>
        /// Get Projects with Pagination and Filtering
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<GetAllProjects> GetProjectsAsync(GetProjectRequest request)
        {
            var cacheKey = "GetProjectsAsync";
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

        /// <summary>
        /// Get Project Details by Id
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet("{projectId}")]
        public async Task<GetProjectDetails> GetProjectByIdAsync(int projectId)
        {
            var cacheKey = $"GetProjectByIdAsync_{projectId}";

            return await _cacheService.GetOrCreateAsync(cacheKey, async () =>
            {
                var project = await _dbContext.Projects
                    .Include(p => p.TeamMembers)
                    .FirstOrDefaultAsync(p => p.Id == projectId);

                if (project == null)
                {
                    return null!;
                }

                var response = new GetProjectDetails
                {
                    Id = project.Id,
                    Name = project.Name,
                    Description = project.Description,
                    StartDate = System.TimeZoneInfo.ConvertTimeFromUtc(project.StartDate, System.TimeZoneInfo.FindSystemTimeZoneById(project.TimeZone)),
                    EndDate = System.TimeZoneInfo.ConvertTimeFromUtc(project.EndDate, System.TimeZoneInfo.FindSystemTimeZoneById(project.TimeZone)),
                    TeamMembers = project.TeamMembers.Select(tm => new UserResponse
                    {
                        Id = tm.Id,
                        Username = tm.Username,
                        Email = tm.Email,
                    }).ToList()
                };
                return response;

            }, expirationInMinutes: 5);
        }

        /// <summary>
        /// Create Task
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(Roles = $"{UserType.MANAGER},{UserType.ADMIN}")]
        [HttpPost]
        public Response CreateTask(CreateTaskRequest request)
        {
            var response = new Response();
            var project = _dbContext.Projects.Include(p => p.Status).FirstOrDefaultAsync(p => p.Id == request.ProjectId).Result;
            if (project == null)
            {
                response.IsSuccess = false;
                response.Error = ErrorMessages.ProjectError;
                return response;
            }

            ProjectTask newTask = new()
            {
                AssignedTo = request.AssignedTo,
                Name = request.Name,
                Description = request.Description,
                Type = request.Type,
                Duration = request.Duration,
                ProjectId = request.ProjectId,
                CreatedAt = DateTime.UtcNow,
                Project = project,
                CreatedById = Convert.ToInt32(_httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)),
                Status = project.Status.Where(s => s.IsDefault).FirstOrDefault()!.StatusId,
            };
            _dbContext.ProjectTasks.Add(newTask);
            _dbContext.SaveChanges();
            response.IsSuccess = true;
            response.Message = "Task" + SuccessMessages.CreationSuccess;
            return response;
        }

        /// <summary>
        /// Create Project Status
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(Roles = $"{UserType.MANAGER},{UserType.ADMIN}")]
        [HttpPost]
        public Response CreateProjectStatus(CreateProjectStatus request)
        {
            var response = new Response();
            var project = _dbContext.Projects.FirstOrDefaultAsync(p => p.Id == request.ProjectId).Result;
            if (project == null)
            {
                response.IsSuccess = false;
                response.Message = ErrorMessages.ProjectError;
                return response;
            }
            ProjectStatuses newStatus = new()
            {
                Status = request.Status,
                IsDefault = request.IsDefault,
                ProjectId = request.ProjectId,
                Project = project,
            };
            _dbContext.Statuses.Add(newStatus);
            _dbContext.SaveChanges();
            response.IsSuccess = true;
            response.Message = "Status " + SuccessMessages.CreationSuccess;
            return response;
        }
        /// <summary>
        /// Delete Project Status
        /// </summary>
        /// <param name="statusId"></param>
        /// <returns></returns>
        [HttpDelete("{statusId}")]
        public Response DeleteProjectStatus(int statusId)
        {
            var response = new Response();
            var status = _dbContext.Statuses.FirstOrDefaultAsync(s => s.StatusId == statusId).Result;
            if (status == null)
            {
                response.IsSuccess = false;
                response.Error = "Status " + ErrorMessages.NotFound;
                return response;
            }
            _dbContext.Statuses.Remove(status);
            _dbContext.SaveChanges();
            response.IsSuccess = true;
            response.Message = "Status " + SuccessMessages.DeleteSuccess;
            return response;
        }

        /// <summary>
        /// Edit Project Status
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(Roles = $"{UserType.MANAGER},{UserType.ADMIN}")]
        [HttpPatch]
        public Response EditProjectStatus(EditProjectStatus request)
        {
            var response = new Response();
            var status = _dbContext.Statuses.FirstOrDefaultAsync(s => s.StatusId == request.StatusId).Result;
            if (status == null)
            {
                response.IsSuccess = false;
                response.Error = "Status " + ErrorMessages.NotFound;
                return response;
            }
            if (request.Status != null)
            {
                status.Status = request.Status;
            }
            if (request.IsDefault != null)
            {
                status.IsDefault = (bool)request.IsDefault;
            }
            if (request.Position != null)
            {
                status.Position = (int)request.Position;
            }
            _dbContext.Statuses.Update(status);
            _dbContext.SaveChanges();
            response.IsSuccess = true;
            response.Message = "Status " + SuccessMessages.UpdateSuccess;
            return response;
        }

        /// <summary>
        /// To Get the Project Status
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [Authorize(Roles = $"{UserType.ADMIN},{UserType.MANAGER}")]
        [HttpGet("projectId")]
        public Task<List<GetProjectStatus>> GetProjectStatuses(int projectId)
        {
            var response = _dbContext.Statuses.Where(x => x.ProjectId == projectId)
                            .Select(status => new GetProjectStatus()
                            {
                                IsDefault = status.IsDefault,
                                Position = status.Position,
                                Status = status.Status,
                                StatusId = status.StatusId
                            }).ToListAsync();

            return response;
        }

        /// <summary>
        /// To get the Tasks
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet("projectId")]
        public Task<List<GetTask>> GetTasks(int projectId)
        {
            var response = _dbContext.ProjectTasks.Where(x=>x.TaskId == projectId)
                            .Select(task => new GetTask()
                            {
                                Description = task.Description,
                                Name = task.Name,
                                Status = task.Status,
                                TaskId = task.TaskId,
                                Type = task.Type
                            }).ToListAsync();
            return response;
        }

        #endregion

        #region private methods
        #endregion
    }
}