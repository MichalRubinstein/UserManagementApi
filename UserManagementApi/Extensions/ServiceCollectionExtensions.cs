using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserManagementApi.Business;
using UserManagementApi.Data;
using UserManagementApi.Models;
using UserManagementApi.Repositories;

namespace UserManagementApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services, IConfiguration configuration)
    {

        services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("UsersDb"));

        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IUserService, UserService>();

        services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

        return services;
    }
}
