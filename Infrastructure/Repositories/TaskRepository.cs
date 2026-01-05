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
}