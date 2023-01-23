namespace CVManager.Extensions;

public static class AppExtensions
{
    public static void UseCustom404(this IApplicationBuilder app)
    {
        app.Use(async (context, next) =>
        {
            await next();
            if (context.Response.StatusCode == 404)
            {
                context.Request.Path = "/Home/CustomNotFound";
                await next();
            }
        });
    }

}