using System.Collections.Generic;

public record class GetAllProjects
{
    public int Count { get; set; }
    public List<ProjectResponse> Projects { get; set; } = new List<ProjectResponse>();
}

public record class ProjectResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public List<UserResponse> TeamMembers { get; set; } = new List<UserResponse>();
}

public record class UserResponse
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
}