public class GetStatusHistory
{
    public DateTime DateTime { get; set; }
    public string UpdatedBy { get; set; }
    public int FromStatusId { get; set; }
    public string FromStatusName { get; set; }
    public int ToStatusId { get; set; }
    public string ToStatusName { get; set; }
}