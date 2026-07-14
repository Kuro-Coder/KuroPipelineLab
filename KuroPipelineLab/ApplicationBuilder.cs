namespace KuroPipelineLab;

public sealed class ApplicationBuilder
{
    private readonly List<Func<RequestDelegate, RequestDelegate>> _components = [];

    public ApplicationBuilder Use(
        Func<RequestDelegate, RequestDelegate> middleware)
    {
        ArgumentNullException.ThrowIfNull(middleware);

        _components.Add(middleware);

        return this;
    }

    public RequestDelegate Build()
    {
        RequestDelegate pipeline = context =>
        {
            context.ResponseStatusCode = 404;
            context.ResponseBody = "Endpoint not found.";

            return Task.CompletedTask;
        };

        for (var index = _components.Count - 1; index >= 0; index--)
        {
            pipeline = _components[index](pipeline);
        }

        return pipeline;
    }
}