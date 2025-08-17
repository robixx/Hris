using ITC.Hris.Application;
using ITC.Hris.Application.Interface;
using ITC.Hris.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace ITC.Hris.Infrastructure
{
    public static class ServiceInjection
    {
        public static void InjectService(this IServiceCollection services)
        {
            services.AddScoped<JwtConfig>();
            services.AddScoped<IAuth, AuthService>();
            services.AddScoped<IUserProfile, UserProfileService>();

        }
    }
}
