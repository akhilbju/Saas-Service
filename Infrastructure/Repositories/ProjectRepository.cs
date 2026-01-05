using Microsoft.EntityFrameworkCore;

public class ProjectRepository : IProjectRepository
{
    private readonly AppDbContext _dbContext;
    public ProjectRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void AddProject(Project project)
    {
        _dbContext.Projects.Add(project);
        _dbContext.SaveChanges();
    }
    public async Task<(List<Project> Projects, int TotalCount)> GetProjectsAsync(
        string? projectName,
        int pageNumber,
        int rowsPerPage)
    {
        IQueryable<Project> query = _dbContext.Projects
            .Include(p => p.TeamMembers);

        if (!string.IsNullOrWhiteSpace(projectName))
        {
            query = query.Where(x =>
                EF.Functions.Like(x.Name, $"%{projectName}%"));
        }

        int skip = (pageNumber - 1) * rowsPerPage;

        var totalCount = await query.CountAsync();

        var projects = await query
            .Skip(skip)
            .Take(rowsPerPage)
            .ToListAsync();

        return (projects, totalCount);
    }

    public async Task<Project?> GetByIdAsyncIncludeTeamMembers(int projectId)
    {
        return await _dbContext.Projects
            .Include(p => p.TeamMembers)
            .FirstOrDefaultAsync(p => p.Id == projectId);
    }

    public async Task<Project?> GetByIdAsyncIncludeStatus(int projectId)
    {
        return await _dbContext.Projects
            .Include(p => p.Status)
            .FirstOrDefaultAsync(p => p.Id == projectId);
    }

    public async Task<Project?> GetByIdAsync(int projectId)
    {
        return await _dbContext.Projects
            .FirstOrDefaultAsync(p => p.Id == projectId);
    }
}