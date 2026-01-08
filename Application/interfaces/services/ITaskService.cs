public interface ITaskService
{
    Response CreateTask(CreateTaskRequest request);
    Task<List<GetTask>> GetTasks(int projectId);
    Response UpdateTask(UpdateTaskRequest request);
    Task<List<GetStatusHistory>> GetStatusHistoryofTask(int taskId);
    Response DeleteTask(int taskId);
}