namespace KuroPipelineLab;

public static class ApplicationBuilderExtensions
{
    public static ApplicationBuilder Run(
        this ApplicationBuilder app,
        RequestDelegate terminalHandler)
    {
        ArgumentNullException.ThrowIfNull(app);
        ArgumentNullException.ThrowIfNull(terminalHandler);

        return app.Use(_ => terminalHandler);
    }

    public static ApplicationBuilder Map(
        this ApplicationBuilder app,
        string path,
        Action<ApplicationBuilder> configureBranch)
    {
        ArgumentNullException.ThrowIfNull(app);
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(configureBranch);

        var branchBuilder = new ApplicationBuilder();

        configureBranch(branchBuilder);

        var branchPipeline = branchBuilder.Build();

        return app.Use(next => async context =>
        {
            if (IsPathMatch(context.RequestPath, path))
            {
                await branchPipeline(context);

                return;
            }

            await next(context);
        });
    }

    private static bool IsPathMatch(
        string requestPath,
        string branchPath)
    {
        if (requestPath.Equals(
                branchPath,
                StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return requestPath.StartsWith(
            branchPath + "/",
            StringComparison.OrdinalIgnoreCase);
    }
}