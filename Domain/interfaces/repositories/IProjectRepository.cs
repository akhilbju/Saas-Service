public interface IProjectRepository
{
    void AddProject(Project project);
    Task<(List<Project> Projects, int TotalCount)> GetProjectsAsync(
        string? projectName,
        int pageNumber,
        int rowsPerPage);
    Task<Project?> GetByIdAsyncIncludeTeamMembers(int projectId);
    Task<Project?> GetByIdAsyncIncludeStatus(int projectId);
    Task<Project?> GetByIdAsync(int projectId);
}