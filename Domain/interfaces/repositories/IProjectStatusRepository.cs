public interface IProjectStatusRepository
{
    void AddStatus(ProjectStatuses statuses);
    void DeleteStatus(ProjectStatuses statuses);
    ProjectStatuses FindStatus(int StatusId);
    void UpdateStatus(ProjectStatuses statuses);
    List<ProjectStatuses> GetAllStatuses(int projectId);
}