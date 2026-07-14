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
}