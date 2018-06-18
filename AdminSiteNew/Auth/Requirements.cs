using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace AdminSiteNew.Auth
{
    public class Access : AuthorizationHandler<Access>, IAuthorizationRequirement
    {

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, Access requirement)
        {
            Console.WriteLine(context.User.Identity.AuthenticationType);
            context.Succeed(requirement);
        }
    }
}

