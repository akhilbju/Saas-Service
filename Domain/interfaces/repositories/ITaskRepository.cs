public interface ITaskRepository
{
    void AddTask(ProjectTask task);
    Task<List<ProjectTask>> GetAllProjectTask(int projectId);
}