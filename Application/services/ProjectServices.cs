using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

public class ProjectServices : IProjectServices
{
    private readonly IProjectRepository _projectRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _userRepository;
    private readonly ICacheService _cacheService;
    private readonly IProjectStatusRepository _projectStatusRepo;

    public ProjectServices(IProjectRepository projectRepository,
        IHttpContextAccessor httpContextAccessor,
        IUserRepository userRepository,
        IProjectStatusRepository projectStatusRepo,
        ICacheService cacheService)
    {
        _projectRepository = projectRepository;
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
        _cacheService = cacheService;
        _projectStatusRepo = projectStatusRepo;
    }

    public Response CreateProject(CreateProjectRequest request)
    {
        var response = new Response();
        var project = new Project
        {
            Name = request.Name,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow,
            CreatedById = Convert.ToInt32(_httpContextAccessor.HttpContext!.User.FindFirstValue(JwtRegisteredClaimNames.Sub)),
            StartDate = System.TimeZoneInfo.ConvertTimeToUtc(request.StartDate, System.TimeZoneInfo.FindSystemTimeZoneById(request.TimeZone)),
            EndDate = System.TimeZoneInfo.ConvertTimeToUtc(request.EndDate, System.TimeZoneInfo.FindSystemTimeZoneById(request.TimeZone)),
            TimeZone = request.TimeZone,
        };
        foreach (var memberId in request.TeamMemberIds)
        {
            var user = _userRepository.GetUserById(memberId);
            if (user != null)
            {
                project.TeamMembers.Add(user);
            }
        }
        _userRepository.GetUserById(project.CreatedById)!.CreatedProjects.Add(project);
        _projectRepository.AddProject(project);
        _cacheService.Remove("GetProjectsAsync");
        response.IsSuccess = true;
        response.Message = SuccessMessages.ProjectCreationSuccess;
        return response;
    }

    public async Task<GetAllProjects> GetProjectsAsync(
        GetProjectRequest request)
    {
        var cacheKey = $"projects_{request.ProjectName}_{request.PageNumber}_{request.RowsPerPage}";

        return await _cacheService.GetOrCreateAsync(cacheKey, async () =>
        {
            int pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;

            var (projects, totalCount) =
                await _projectRepository.GetProjectsAsync(
                    request.ProjectName,
                    pageNumber,
                    request.RowsPerPage);

            return new GetAllProjects
            {
                Count = totalCount,
                Projects = projects.Select(p => new ProjectResponse
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    TeamMembers = p.TeamMembers.Select(tm => new UserResponse
                    {
                        Id = tm.Id,
                        Username = tm.Username,
                        Email = tm.Email
                    }).ToList()
                }).ToList()
            };
        }, expirationInMinutes: 3);
    }

    public async Task<GetProjectDetails> GetProjectByIdAsync(int projectId)
    {
        var cacheKey = $"GetProjectByIdAsync_{projectId}";

        return await _cacheService.GetOrCreateAsync(cacheKey, async () =>
        {
            var project = await _projectRepository.GetByIdAsyncIncludeTeamMembers(projectId);
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

    public Response CreateProjectStatus(CreateProjectStatus request)
    {
        var response = new Response();
        var project = _projectRepository.GetByIdAsync(request.ProjectId).Result;
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
            Position = request.Position,
            Project = project,
        };
        _projectStatusRepo.AddStatus(newStatus);
        response.IsSuccess = true;
        response.Message = "Status " + SuccessMessages.CreationSuccess;
        return response;
    }
    public Response DeleteProjectStatus(int statusId)
    {
        var response = new Response();
        var status = _projectStatusRepo.FindStatus(statusId);
        if (status == null)
        {
            response.IsSuccess = false;
            response.Error = "Status " + ErrorMessages.NotFound;
            return response;
        }
        _projectStatusRepo.DeleteStatus(status);
        response.IsSuccess = true;
        response.Message = "Status " + SuccessMessages.DeleteSuccess;
        return response;
    }

    public Response EditProjectStatus(EditProjectStatus request)
    {
        var response = new Response();
        var status = _projectStatusRepo.FindStatus(request.StatusId);
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
        _projectStatusRepo.UpdateStatus(status);
        response.IsSuccess = true;
        response.Message = "Status " + SuccessMessages.UpdateSuccess;
        return response;
    }

    public List<GetProjectStatus> GetProjectStatuses(int projectId)
    {
        var statuses = _projectStatusRepo.GetAllStatuses(projectId);
        var response = statuses.Select(status => new GetProjectStatus()
                        {
                            IsDefault = status.IsDefault,
                            Position = status.Position,
                            Status = status.Status,
                            StatusId = status.StatusId
                        }).ToList();

        return response;
    }
}