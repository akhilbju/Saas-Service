using System;
using System.Collections.Generic;

public record class GetTask
{
    public int TaskId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; }
    public int Status { get; set; }
}