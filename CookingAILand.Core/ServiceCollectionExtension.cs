using System.Text;
using CookingAILand.Core.DAL.Entities;
using CookingAILand.Core.DAL.Persistence;
using CookingAILand.Core.DAL.Repositories;
using CookingAILand.Core.Entities;
using CookingAILand.Middleware;
using CookingAILand.Models;
using CookingAILand.Models.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace CookingAILand.Core;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration configuration)
    {
      
        return services;
    }

    public static IApplicationBuilder UseCore(this IApplicationBuilder app)
    {
       
        return app;
    }
}