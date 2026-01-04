public record class Response
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public string Error { get; set; }
};