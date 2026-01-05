public interface IProjectServices
{
    Response CreateProject(CreateProjectRequest request);
    Task<GetAllProjects> GetProjectsAsync(GetProjectRequest request);
    Task<GetProjectDetails> GetProjectByIdAsync(int projectId);
    Response CreateProjectStatus(CreateProjectStatus request);
    Response DeleteProjectStatus(int statusId);
    List<GetProjectStatus> GetProjectStatuses(int projectId);
    Response EditProjectStatus(EditProjectStatus request);
}