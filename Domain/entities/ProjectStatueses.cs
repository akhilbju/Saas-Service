using System.Collections.Generic;

public record class ProjectStatuses
{
    public int ProjectId { get; set; }
    public Project Project { get; set; }
    public string Status { get; set; }
    public int StatusId { get; set; }
    public bool IsDefault { get; set; }
    public int Position { get; set; }   
}
