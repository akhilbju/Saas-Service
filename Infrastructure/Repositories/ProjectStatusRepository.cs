public class ProjectStatusRepository : IProjectStatusRepository
{
    private readonly AppDbContext _context;
    public ProjectStatusRepository(AppDbContext context)
    {
        _context = context;
    }
    public void AddStatus(ProjectStatuses statuses)
    {
        _context.Statuses.Add(statuses);
        _context.SaveChanges();
    }

    public void DeleteStatus(ProjectStatuses statuses)
    {
        _context.Statuses.Remove(statuses);
        _context.SaveChanges();
    }

    public ProjectStatuses FindStatus(int statusId)
    {
        return _context.Statuses.FirstOrDefault(x=>x.StatusId == statusId);
    }

    public void UpdateStatus(ProjectStatuses status)
    {
        _context.Statuses.Update(status);
        _context.SaveChanges();
    }

    public List<ProjectStatuses> GetAllStatuses(int ProjectId)
    {
        return _context.Statuses.Where(x=>x.ProjectId == ProjectId).ToList();
    }
}