namespace KuroPipelineLab;

internal static class Program
{
    private static async Task Main()
    {
        var builder = new ApplicationBuilder();

        builder.Use(next => async context =>
        {
            Console.WriteLine("1. Logging started.");

            await next(context);

            Console.WriteLine("6. Logging finished.");
        });

        builder.Use(next => async context =>
        {
            Console.WriteLine("2. Authorization started.");

            var isAuthenticated = true;

            if (!isAuthenticated)
            {
                context.ResponseStatusCode = 401;
                context.ResponseBody = "Unauthorized";

                return;
            }

            await next(context);

            Console.WriteLine("5. Authorization finished.");
        });

        builder.Use(next => async context =>
        {
            var startedAt = DateTime.UtcNow;

            await next(context);

            var finishedAt = DateTime.UtcNow;
            var duration = finishedAt - startedAt;

            Console.WriteLine(
                $"Request duration: {duration.TotalMilliseconds} ms");
        });

        builder.Use(next => async context =>
        {
            Console.WriteLine("3. Endpoint started.");

            await Task.Delay(500);

            context.ResponseStatusCode = 200;
            context.ResponseBody = "Products list";

            Console.WriteLine("4. Endpoint finished.");

            await Task.CompletedTask;
        });

        var pipeline = builder.Build();

        var context = new PipelineContext
        {
            RequestPath = "/products",
            RequestMethod = "GET"
        };

        await pipeline(context);

        Console.WriteLine();
        Console.WriteLine($"Status code: {context.ResponseStatusCode}");
        Console.WriteLine($"Response body: {context.ResponseBody}");
    }
}