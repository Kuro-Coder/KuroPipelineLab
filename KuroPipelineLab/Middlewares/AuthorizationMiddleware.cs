namespace KuroPipelineLab.Middlewares;

public sealed class AuthorizationMiddleware
{
    private readonly RequestDelegate _next;

    public AuthorizationMiddleware(RequestDelegate next)
    {
        ArgumentNullException.ThrowIfNull(next);

        _next = next;
    }

    public async Task InvokeAsync(PipelineContext context)
    {
        var isAuthenticated =
            context.Items.TryGetValue("IsAuthenticated", out var value) &&
            value is true;

        if (!isAuthenticated)
        {
            context.ResponseStatusCode = 401;
            context.ResponseBody = "Unauthorized";

            Console.WriteLine(
                "[Authorization] Request was rejected.");

            return;
        }

        Console.WriteLine(
            "[Authorization] Request was accepted.");

        await _next(context);
    }
}