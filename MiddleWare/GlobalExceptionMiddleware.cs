using System.Diagnostics;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next,
                                     ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var trace = new StackTrace(ex, true);
            var frame = trace.GetFrame(0);

            var method = frame?.GetMethod();
            var methodName = method?.Name;
            var className = method?.DeclaringType?.FullName;
            var lineNumber = frame?.GetFileLineNumber();

            _logger.LogError(ex,
                "Unhandled exception in {Class}.{Method} at line {Line}",
                className, methodName, lineNumber);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            Response response = new()
            {
                IsSuccess = false,
                Message = "An unexpected error occurred.",
                Error = context.TraceIdentifier
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
