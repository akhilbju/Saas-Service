using System;
using System.Collections.Generic;

public record class GetProjectDetails
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<UserResponse> TeamMembers { get; set; } = new List<UserResponse>();
    public bool IsCompleted { get; set; }
}

