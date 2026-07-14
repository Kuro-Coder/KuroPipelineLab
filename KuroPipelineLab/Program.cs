namespace KuroPipelineLab;

internal static class Program
{
    private static async Task Main()
    {
        var app = new ApplicationBuilder();

        app.Use(next => async context =>
        {
            Console.WriteLine(
                $"Request started: {context.RequestMethod} {context.RequestPath}");

            await next(context);

            Console.WriteLine(
                $"Request finished: {context.ResponseStatusCode}");
        });

        app.Map("/products", productsApp =>
        {
            productsApp.Use(next => async context =>
            {
                Console.WriteLine("Products middleware started.");

                await next(context);

                Console.WriteLine("Products middleware finished.");
            });

            productsApp.Run(async context =>
            {
                context.ResponseStatusCode = 200;
                context.ResponseBody = "Products branch response";

                await Task.CompletedTask;
            });
        });

        app.Map("/orders", ordersApp =>
        {
            ordersApp.Use(next => async context =>
            {
                Console.WriteLine("Orders middleware started.");

                await next(context);

                Console.WriteLine("Orders middleware finished.");
            });

            ordersApp.Run(async context =>
            {
                context.ResponseStatusCode = 200;
                context.ResponseBody = "Orders branch response";

                await Task.CompletedTask;
            });
        });

        app.Map("/admin", adminApp =>
        {
            adminApp.Use(next => async context =>
            {
                var isAdmin = context.Items.TryGetValue(
                    "IsAdmin",
                    out var value)
                    && value is true;

                if (!isAdmin)
                {
                    context.ResponseStatusCode = 403;
                    context.ResponseBody = "Forbidden";

                    return;
                }

                await next(context);
            });

            adminApp.Run(async context =>
            {
                context.ResponseStatusCode = 200;
                context.ResponseBody = "Welcome to admin panel";

                await Task.CompletedTask;
            });
        });

        app.Run(async context =>
        {
            context.ResponseStatusCode = 404;
            context.ResponseBody = $"No endpoint found for {context.RequestPath}";

            await Task.CompletedTask;
        });

        var pipeline = app.Build();

        await ExecuteRequest(
            pipeline,
            requestMethod: "GET",
            requestPath: "/products");

        await ExecuteRequest(
            pipeline,
            requestMethod: "POST",
            requestPath: "/orders");

        await ExecuteRequest(
            pipeline,
            requestMethod: "GET",
            requestPath: "/customers");
        
        var context = new PipelineContext
        {
            RequestMethod = "GET",
            RequestPath = "/admin"
        };
        context.Items["IsAdmin"] = true;
        await pipeline(context);

    }

    private static async Task ExecuteRequest(
        RequestDelegate pipeline,
        string requestMethod,
        string requestPath)
    {
        Console.WriteLine();
        Console.WriteLine(new string('-', 50));

        var context = new PipelineContext
        {
            RequestMethod = requestMethod,
            RequestPath = requestPath
        };

        await pipeline(context);

        Console.WriteLine($"Response status: {context.ResponseStatusCode}");
        Console.WriteLine($"Response body: {context.ResponseBody}");
    }
}