using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.Features;

public record class Project
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<User> TeamMembers { get; set; } = new List<User>();
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public int CreatedById { get; set; }
    public User CreatedBy { get; set; } 
    [Required]
    public string TimeZone { get; set; } 
    public List<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
    public ICollection<ProjectStatuses> Status { get; set; }  = new List<ProjectStatuses>();
}