namespace KuroPipelineLab.Middlewares;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        ArgumentNullException.ThrowIfNull(next);

        _next = next;
    }

    public async Task InvokeAsync(PipelineContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            context.ResponseStatusCode = 500;
            context.ResponseBody = "An unexpected error occurred.";

            Console.WriteLine(
                $"[Exception] {exception.GetType().Name}: " +
                exception.Message);
        }
    }
}