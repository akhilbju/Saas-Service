using Microsoft.EntityFrameworkCore;
using Saas_Auth_Service.Migrations;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectTask> ProjectTasks { get; set; }
    public DbSet<ProjectStatuses> Statuses { get; set; }
    public DbSet<TaskStatusHistory> TaskStatusHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
                    .Property(u => u.Id)
                    .ValueGeneratedOnAdd();

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.TokenId);
            entity.Property(e => e.TokenId).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<Project>()
            .HasMany(p => p.TeamMembers)
            .WithMany(u => u.Projects);

        modelBuilder.Entity<Project>()
            .HasOne(p => p.CreatedBy)
            .WithMany(u => u.CreatedProjects)
            .HasForeignKey(p => p.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ProjectTask>(x =>
        {
            x.HasKey(t => t.TaskId);
            x.Property(t => t.TaskId).ValueGeneratedOnAdd();
        });


        modelBuilder.Entity<ProjectTask>()
            .HasOne(p => p.Project)
            .WithMany(t => t.Tasks)
            .HasForeignKey(p => p.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProjectStatuses>(x =>
        {
            x.HasKey(s => s.StatusId);
            x.Property(s => s.StatusId).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<ProjectStatuses>()
            .HasOne(p => p.Project)
            .WithMany(s => s.Status)
            .HasForeignKey(p => p.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TaskStatusHistory>()
        .HasOne(th => th.Task)
        .WithMany(t => t.TaskHistories)
        .HasForeignKey(th => th.TaskId)
        .OnDelete(DeleteBehavior.Cascade);
    }

}
