using System;
using System.Collections.Generic;

public record class ProjectTask
{
    public int TaskId { get; set; }
    public Project Project { get; set; }
    public int ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public long Duration { get; set; }
    public string Type { get; set; }
    public List<int> AssignedTo { get; set; } = new List<int>();
    public int Status { get; set; }
    public int CreatedById { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<TaskStatusHistory> TaskHistories { get; set; }

}