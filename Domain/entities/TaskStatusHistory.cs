public class TaskStatusHistory
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public int FromStatusId { get; set; }
    public int ToStatusId { get; set; }
    public DateTime ChangedAt { get; set; }
    public int ChangedBy { get; set; }

}