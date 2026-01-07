public interface ITaskService
{
    Response CreateTask(CreateTaskRequest request);
    Task<List<GetTask>> GetTasks(int projectId);
    Response UpdateTask(UpdateTaskRequest request);
    List<GetStatusHistory> GetStatusHistoryofTask(int TaskId);
}