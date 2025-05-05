public class ProcessingModeValidationMiddleware
{
    private readonly RequestDelegate _next;

    public ProcessingModeValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLower();

        if (path != null && (path.StartsWith("/swagger") || path.StartsWith("/favicon") || path.Contains(".css") || path.Contains(".js")))
        {
            await _next(context);
            return;
        }

        var mode = context.Request.Headers["X-Processing-Mode"].FirstOrDefault()?.ToLower();

        if (string.IsNullOrWhiteSpace(mode))
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync("Missing required header: X-Processing-Mode");
            return;
        }

        if (mode != "db" && mode != "kafka")
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync($"Invalid X-Processing-Mode: '{mode}'. Only 'db' or 'kafka' are allowed.");
            return;
        }

        await _next(context);
    }
}
