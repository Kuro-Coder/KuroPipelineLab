namespace KuroPipelineLab;

internal static class Program
{
    private static async Task Main()
    {
        var app = new ApplicationBuilder();

        app.Use(next => async context =>
        {
            Console.WriteLine("Logging started.");

            await next(context);

            Console.WriteLine("Logging finished.");
        });

        app.Use(next => async context =>
        {
            Console.WriteLine("Authorization started.");

            var isAuthenticated = true;

            if (!isAuthenticated)
            {
                context.ResponseStatusCode = 401;
                context.ResponseBody = "Unauthorized";

                return;
            }

            await next(context);

            Console.WriteLine("Authorization finished.");
        });

        app.Run(async context =>
        {
            Console.WriteLine("Terminal endpoint started.");

            await Task.Delay(300);

            context.ResponseStatusCode = 200;
            context.ResponseBody = "Products list";

            Console.WriteLine("Terminal endpoint finished.");
        });

        var pipeline = app.Build();

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