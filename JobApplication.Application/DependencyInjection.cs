using JobApplication.Application.Features.Jobs.Commands.CloseJob;
using JobApplication.Application.Interfaces;
using JobApplication.Application.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace JobApplication.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IJobService, JobService>();
            services.AddScoped<IJobCandidateApplicationService, JobCandidateApplicationService>();
            services.AddScoped<IAuthService, AuthService>();

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CloseJobCommandHandler>());

            return services;
        }
    }
}
