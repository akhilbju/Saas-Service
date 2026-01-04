using System;
using System.Collections.Generic;

public record class CreateTaskRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public long Duration { get; set; }
    public string Type { get; set; }
    public List<int> AssignedTo { get; set; } = new List<int>();
    public int ProjectId { get; set; }

}