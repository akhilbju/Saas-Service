public interface ITaskRepository
{
    void AddTask(ProjectTask task);
    Task<List<ProjectTask>> GetAllProjectTask(int projectId);
    void UpdateTask(ProjectTask task);
    ProjectTask GetTaskById(int taskId);
    void DeleteTask(ProjectTask Task);
}