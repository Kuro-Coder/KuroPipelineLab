namespace KuroPipelineLab;

public sealed class PipelineContext
{
    public string RequestPath { get; set; } = "/";

    public string RequestMethod { get; set; } = "GET";

    public int ResponseStatusCode { get; set; } = 200;

    public string? ResponseBody { get; set; }

    public Dictionary<string, object?> Items { get; } = [];
}