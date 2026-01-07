public interface ITaskHistoryRepository
{
    void AddHistory(TaskStatusHistory History);
    List<TaskStatusHistory> GetStatusHistoryOfTask(int TaskId);
}