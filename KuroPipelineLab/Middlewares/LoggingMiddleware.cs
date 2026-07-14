namespace KuroPipelineLab.Middlewares;

public sealed class LoggingMiddleware
{
    private readonly RequestDelegate _next;

    public LoggingMiddleware(RequestDelegate next)
    {
        ArgumentNullException.ThrowIfNull(next);

        _next = next;
    }

    public async Task InvokeAsync(PipelineContext context)
    {
        Console.WriteLine(
            $"[Logging] Request started: " +
            $"{context.RequestMethod} {context.RequestPath}");

        await _next(context);

        Console.WriteLine(
            $"[Logging] Request finished: " +
            $"{context.ResponseStatusCode}");
    }
}