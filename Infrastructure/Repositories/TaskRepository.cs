using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _context;

    public TaskRepository(AppDbContext context)
    {
        _context = context;
    }

    public void AddTask(ProjectTask task)
    {
        _context.ProjectTasks.Add(task);
        _context.SaveChanges();
    }

    public async Task<List<ProjectTask>> GetAllProjectTask(int projectId)
    {
       return await _context.ProjectTasks.Where(x=>x.ProjectId == projectId).ToListAsync();
    }

    public void UpdateTask(ProjectTask task)
    {
         _context.ProjectTasks.Update(task);
         _context.SaveChanges();
    }

    public  ProjectTask GetTaskById(int taskId)
    {
        return _context.ProjectTasks.Where(x=>x.TaskId == taskId).FirstOrDefault();
    }

    public void DeleteTask(ProjectTask Task)
    {
        _context.ProjectTasks.Remove(Task);
        _context.SaveChanges();
    }

}