namespace Error;

public class DefaultErrorHandler(RequestDelegate next)
{
    private RequestDelegate _next = next;
    public async Task Invoke(HttpContext context)
    {
        try{
            await _next(context);
        }
        catch(CustomException ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            //var errorMessage = new { error =ex.Message };
            await context.Response.WriteAsync(ex.Message);
        }
        catch(Exception ex)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            //var errorMessage = new { error =ex.Message };
            await context.Response.WriteAsync(ex.Message);
        }

    }
}