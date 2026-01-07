using System.Threading.Tasks;

public class TaskHistoryRepository : ITaskHistoryRepository
{
    private readonly AppDbContext  _context;

    public TaskHistoryRepository(AppDbContext context)
    {
        this._context = context;
    }

    public void AddHistory(TaskStatusHistory History)
    {
        this._context.TaskStatusHistories.AddAsync(History);
        this._context.SaveChanges();
    }

    public List<TaskStatusHistory> GetStatusHistoryOfTask(int TaskId)
    {
        return  _context.TaskStatusHistories.Where(x=>x.TaskId == TaskId).ToList();
    }
}