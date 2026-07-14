using System.Diagnostics;

namespace KuroPipelineLab.Middlewares;

public sealed class ExecutionTimeMiddleware
{
    private readonly RequestDelegate _next;

    public ExecutionTimeMiddleware(RequestDelegate next)
    {
        ArgumentNullException.ThrowIfNull(next);

        _next = next;
    }

    public async Task InvokeAsync(PipelineContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            Console.WriteLine(
                $"[ExecutionTime] Request duration: " +
                $"{stopwatch.ElapsedMilliseconds} ms");
        }
    }
}