using KuroPipelineLab.Middlewares;

namespace KuroPipelineLab;

internal static class Program
{
    private static async Task Main()
    {
        var app = new ApplicationBuilder();

        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseMiddleware<LoggingMiddleware>();
        app.UseMiddleware<ExecutionTimeMiddleware>();
        app.UseMiddleware<AuthorizationMiddleware>();

        app.Run(async context =>
        {
            Console.WriteLine("[Endpoint] Endpoint started.");

            await Task.Delay(300);

            context.ResponseStatusCode = 200;
            context.ResponseBody = "Products list";

            Console.WriteLine("[Endpoint] Endpoint finished.");
        });

        var pipeline = app.Build();

        await ExecuteRequest(
            pipeline,
            requestMethod: "GET",
            requestPath: "/products",
            isAuthenticated: true);

        await ExecuteRequest(
            pipeline,
            requestMethod: "GET",
            requestPath: "/products",
            isAuthenticated: false);
    }

    private static async Task ExecuteRequest(
        RequestDelegate pipeline,
        string requestMethod,
        string requestPath,
        bool isAuthenticated)
    {
        Console.WriteLine();
        Console.WriteLine(new string('-', 60));

        var context = new PipelineContext
        {
            RequestMethod = requestMethod,
            RequestPath = requestPath
        };

        context.Items["IsAuthenticated"] = isAuthenticated;

        await pipeline(context);

        Console.WriteLine(
            $"Response status: {context.ResponseStatusCode}");

        Console.WriteLine(
            $"Response body: {context.ResponseBody}");
    }
}