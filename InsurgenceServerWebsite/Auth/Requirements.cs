using Microsoft.AspNet.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InsurgenceServerWebsite
{
    public class Access : AuthorizationHandler<Access>, IAuthorizationRequirement
    {

        protected override void Handle(AuthorizationContext context, Access requirement)
        {
            Console.WriteLine(context.User.Identity.AuthenticationType);
            context.Succeed(requirement);
        }
    }
}

