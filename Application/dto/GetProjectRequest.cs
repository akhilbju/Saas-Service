public record class GetProjectRequest
{
    public string ProjectName { get; set; }
    public int RowsPerPage { get; set; }
    public int PageNumber { get; set; }
}