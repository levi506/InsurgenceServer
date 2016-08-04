using Microsoft.AspNet.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading;
using Microsoft.AspNet.Authentication.Google;
using Microsoft.AspNet.Authentication.Cookies;
using InsurgenceServerWebsite.Auth;
using Microsoft.AspNet.Authentication.OAuth;
using System.Threading.Tasks;
using Microsoft.Extensions.WebEncoders;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Authentication;
using System.Linq;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Authorization;

namespace InsurgenceServerWebsite
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddIdentity<User, Role>();
            services.AddAuthentication(o =>
                o.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme );
            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    "Access",
                    authBuilder =>
                    {
                        authBuilder.RequireClaim("Access", "Allowed");
                    });
            });
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(LogLevel.Warning);

            app.UseStaticFiles();

            app.UseDeveloperExceptionPage();

            app.UseIdentity();

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                LoginPath = new PathString("/login")
            });

            var options = new GoogleOptions
            {
                ClientId = "978297029125-doscbkcd7p9o7lqa4f9cro5nb2fetfth.apps.googleusercontent.com",
                ClientSecret = "XKJMty6DaT0z7b7kBxlLyrn_",
                Events = new OAuthEvents()
                {
                    OnRemoteError = ctx =>
                    {
                        ctx.Response.Redirect("/error?ErrorMessage=" + UrlEncoder.Default.UrlEncode(ctx.Error.Message));
                        ctx.HandleResponse();
                        return Task.FromResult(0);
                    },
                    OnCreatingTicket = x =>
                    {
                        if (DatabaseSpace.Database.RegisterWebAdmin(x.Identity.GetUserId(), x.Identity.GetUserName()))
                        {
                            x.Identity.AddClaim(new Claim("Access", "Allowed"));
                            return Task.FromResult(0);
                        }
                        else
                        {
                            return Task.FromResult(0);
                        }
                    }
                },
            };
            app.UseGoogleAuthentication(options);

            

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute(
                    name: "user",
                    template: "{controller}/{action}/{id}",
                    defaults: new { controller = "Admin", action = "Users", id = "" }
                    );
            });

            app.Map("/login", signoutApp =>
            {
                signoutApp.Run(async context =>
                {
                    var authType = context.Request.Query["authscheme"];
                    if (!string.IsNullOrEmpty(authType))
                    {
                        // By default the client will be redirect back to the URL that issued the challenge (/login?authtype=foo),
                        // send them to the home page instead (/).
                        await context.Authentication.ChallengeAsync(authType, new AuthenticationProperties() { RedirectUri = "/" });
                        return;
                    }

                    context.Response.ContentType = "text/html";
                    await context.Response.WriteAsync("<html><body>");
                    await context.Response.WriteAsync("Choose an authentication scheme: <br>");
                    foreach (var type in context.Authentication.GetAuthenticationSchemes())
                    {
                        if (type.DisplayName != null)
                            await context.Response.WriteAsync("<a href=\"?authscheme=" + type.AuthenticationScheme + "\">" + (type.DisplayName ?? "(suppressed)") + "</a><br>");
                    }
                    await context.Response.WriteAsync("</body></html>");
                });
            });
            app.Map("/logout", signoutApp =>
            {
                signoutApp.Run(async context =>
                {
                    context.Response.ContentType = "text/html";
                    await context.Authentication.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    await context.Response.WriteAsync("<html><body>");
                    await context.Response.WriteAsync("You have been logged out. Goodbye " + context.User.Identity.Name + "<br>");
                    await context.Response.WriteAsync("<a href=\"/\">Home</a>");
                    await context.Response.WriteAsync("</body></html>");
                });
            });
        }
    }
}