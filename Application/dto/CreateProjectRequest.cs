using System;
using System.Collections.Generic;

public record class CreateProjectRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<int> TeamMemberIds { get; set; } = new List<int>();
    public string TimeZone { get; set; }

}