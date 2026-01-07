public record class UpdateTaskRequest
{
    public int TaskId { get; set; }
    public string TaskName { get; set; }
    public int? Status { get; set; }
    public string Description { get; set; }
    public List<int> Assignees { get; set; }
    public int? Duration { get; set; }

}