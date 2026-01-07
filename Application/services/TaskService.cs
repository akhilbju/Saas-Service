using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

public class TaskService : ITaskService
{
    private readonly IProjectRepository _projectRepository;
    private readonly ITaskRepository _taskRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICacheService _cacheService;
    private readonly ITaskHistoryRepository _taskHistoryRepository;
    private readonly IProjectStatusRepository _statusRepository;
    private readonly IUserRepository _userRepository;


    public TaskService(IProjectRepository projectRepository,
                       ITaskRepository taskRepository, IHttpContextAccessor httpContextAccessor, ICacheService cacheService, ITaskHistoryRepository taskHistoryRepository,
                       IProjectStatusRepository statusRepository,IUserRepository userRepository)
    {
        _projectRepository = projectRepository;
        _taskRepository = taskRepository;
        _httpContextAccessor = httpContextAccessor;
        _cacheService = cacheService;
        _taskHistoryRepository = taskHistoryRepository;
        _projectRepository = projectRepository;
        _userRepository = userRepository;
    }

    public Response CreateTask(CreateTaskRequest request)
    {
        var response = new Response();
        var project = _projectRepository.GetByIdAsyncIncludeStatus(request.ProjectId).Result;
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
        _taskRepository.AddTask(newTask);
        _cacheService.Remove($"getTasks_{request.ProjectId}");
        response.IsSuccess = true;
        response.Message = "Task" + SuccessMessages.CreationSuccess;
        return response;
    }

    public async Task<List<GetTask>> GetTasks(int projectId)
    {
        return await _cacheService.GetOrCreateAsync(
            $"getTasks_{projectId}",
            async () =>
            {
                var tasks = await _taskRepository.GetAllProjectTask(projectId);

                return tasks.Select(x => new GetTask
                {
                    Description = x.Description,
                    Name = x.Name,
                    Status = x.Status,
                    TaskId = x.TaskId,
                    Type = x.Type
                }).ToList();
            },
            3
        );
    }

    public Response UpdateTask(UpdateTaskRequest request)
    {
        Response response = new();
        var task = this._taskRepository.GetTaskById(request.TaskId);
        if (task == null)
        {
            response.Message = "Task" + ErrorMessages.NotFound;
            return response;
        }
        if (string.IsNullOrEmpty(request.TaskName)) task.Name = request.TaskName;
        if (string.IsNullOrEmpty(request.Description)) task.Description = request.Description;
        if (request.Status != null)
        {
            TaskStatusHistory newHistory = new()
            {
                FromStatusId = task.Status,
                ToStatusId = (int)request.Status,
                TaskId = task.TaskId,
                ChangedAt = DateTime.UtcNow,
                ChangedBy = Convert.ToInt32(_httpContextAccessor.HttpContext!.User.FindFirstValue(JwtRegisteredClaimNames.Sub)),
            };
            _taskHistoryRepository.AddHistory(newHistory);
            task.Status = (int)request.Status;


        }
        if (request.Duration != null) task.Duration = (int)request.Duration;
        task.AssignedTo = request.Assignees;
        _taskRepository.UpdateTask(task);
        response.IsSuccess = true;
        response.Message = "Task" + SuccessMessages.UpdateSuccess;
        return response;
    }

    public List<GetStatusHistory> GetStatusHistoryofTask(int TaskId)
    {
        var response = new List<GetStatusHistory>();
        var histories = _taskHistoryRepository.GetStatusHistoryOfTask(TaskId);
        foreach(var history in histories)
        {
            response.Add(new GetStatusHistory()
            {
                DateTime = history.ChangedAt,
                FromStatusId = history.FromStatusId,
                ToStatusId = history.ToStatusId,
                FromStatusName = _statusRepository.FindStatus(history.FromStatusId).Status,
                ToStatusName = _statusRepository.FindStatus(history.ToStatusId).Status,
                UpdatedBy =  _userRepository.GetUserById(history.ChangedBy).Username
            });
        }
        return response;
    }

}