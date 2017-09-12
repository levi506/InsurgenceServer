using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminSiteNew
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

