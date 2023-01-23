using CVManager.DAL;
using CVManager.Services;
using Microsoft.EntityFrameworkCore;

namespace CVManager.Extensions;

public static class BuilderExtensions
{
    public static void ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<DataContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("sqlConnection"),
                b => b.MigrationsAssembly("CVManager")));
        //τα migrations να γίνουν εδώ και όχι στο CVManager.DAL
    }

    public static void ConfigureUoW(this IServiceCollection services)
    {
        services.AddScoped<IUoW, UoW>();
    }

    public static void ConfigureFileManager(this IServiceCollection services)
    {
        services.AddSingleton<IFileManager, FileManager>();
    }

}