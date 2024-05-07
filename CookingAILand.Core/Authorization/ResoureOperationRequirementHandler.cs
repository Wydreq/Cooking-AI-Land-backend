using System.Security.Claims;
using CookingAILand.Core.DAL.Entities;
using Microsoft.AspNetCore.Authorization;

namespace CookingAILand.Core.Authorization;

public class ResourceOperationRequirementHandler : AuthorizationHandler<ResourceOperationRequirement, Cookbook>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        ResourceOperationRequirement requirement,
        Cookbook cookbook)
    {
        if (requirement.ResourceOperation == ResourceOperation.Read ||
            requirement.ResourceOperation == ResourceOperation.Create)
        {
            context.Succeed(requirement);
        }

        var userId = context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value;
        if (cookbook.CreatedById == Guid.Parse(userId))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}