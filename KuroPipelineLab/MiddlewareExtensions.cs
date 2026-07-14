using System.Reflection;

namespace KuroPipelineLab;

public static class MiddlewareExtensions
{
    public static ApplicationBuilder UseMiddleware<TMiddleware>(
        this ApplicationBuilder app)
        where TMiddleware : class
    {
        ArgumentNullException.ThrowIfNull(app);

        var middlewareType = typeof(TMiddleware);

        var constructor = middlewareType.GetConstructor(
            [typeof(RequestDelegate)]);

        if (constructor is null)
        {
            throw new InvalidOperationException(
                $"Middleware '{middlewareType.Name}' must have a public " +
                $"constructor with one '{nameof(RequestDelegate)}' parameter.");
        }

        var invokeMethod = middlewareType.GetMethod(
            "InvokeAsync",
            BindingFlags.Instance | BindingFlags.Public,
            binder: null,
            types: [typeof(PipelineContext)],
            modifiers: null);

        if (invokeMethod is null)
        {
            throw new InvalidOperationException(
                $"Middleware '{middlewareType.Name}' must have a public " +
                $"'InvokeAsync({nameof(PipelineContext)})' method.");
        }

        if (invokeMethod.ReturnType != typeof(Task))
        {
            throw new InvalidOperationException(
                $"Method '{middlewareType.Name}.InvokeAsync' must return Task.");
        }

        return app.Use(next =>
        {
            var middlewareInstance = constructor.Invoke([next]);

            var invokeDelegate =
                (RequestDelegate)invokeMethod.CreateDelegate(
                    typeof(RequestDelegate),
                    middlewareInstance);

            return invokeDelegate;
        });
    }
}