using ITC.Hris.Application;
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

        }
    }
}
