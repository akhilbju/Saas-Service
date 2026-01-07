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
        /// Project Service
        /// </summary>
        private readonly IProjectServices _projectServices;
        private readonly ITaskService _taskService;

        public ProjectController(IProjectServices projectServices, ITaskService taskService)
        {
            _projectServices = projectServices;
            _taskService = taskService;
        }

        /// <summary>
        /// Create Project
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(Roles = $"{UserType.MANAGER},{UserType.ADMIN}")]
        [HttpPost]
        public Response CreateProject(CreateProjectRequest request)
        {
            return _projectServices.CreateProject(request);
        }

        /// <summary>
        /// Get Projects with Pagination and Filtering
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<GetAllProjects> GetProjects(GetProjectRequest request)
        {
            return await _projectServices.GetProjectsAsync(request);
        }

        /// <summary>
        /// Get Project Details by Id
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet("{projectId}")]
        public async Task<GetProjectDetails> GetProjectDetails(int projectId)
        {
            return await _projectServices.GetProjectByIdAsync(projectId);
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
            return _taskService.CreateTask(request);
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
            return _projectServices.CreateProjectStatus(request);
        }
        /// <summary>
        /// Delete Project Status
        /// </summary>
        /// <param name="statusId"></param>
        /// <returns></returns>
        [HttpDelete("{statusId}")]
        public Response DeleteProjectStatus(int statusId)
        {
           return _projectServices.DeleteProjectStatus(statusId);
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
           return _projectServices.EditProjectStatus(request);
        }

        /// <summary>
        /// To Get the Project Status
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [Authorize(Roles = $"{UserType.ADMIN},{UserType.MANAGER}")]
        [HttpGet("projectId")]
        public List<GetProjectStatus> GetProjectStatuses(int projectId)
        {
            return _projectServices.GetProjectStatuses(projectId);
        }

        /// <summary>
        /// To get the Tasks
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet("projectId")]
        public Task<List<GetTask>> GetTasks(int projectId)
        {
            return _taskService.GetTasks(projectId);
        }

        /// <summary>
        /// To Edit the Task
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public Response UpdateTask(UpdateTaskRequest request)
        {
            return _taskService.UpdateTask(request);
        }

        /// <summary>
        /// To get History of Task Status
        /// </summary>
        /// <param name="TaskId"></param>
        /// <returns></returns>
        [HttpGet]
        public List<GetStatusHistory> GetTaskStatusHistories(int TaskId)
        {
            return _taskService.GetStatusHistoryofTask(TaskId);
        }

    }
}