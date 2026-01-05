public interface ITaskService
{
    Response CreateTask(CreateTaskRequest request);
    Task<List<GetTask>> GetTasks(int projectId);
}