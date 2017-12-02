using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AdminSiteNew.Auth;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Text.Encodings.Web;
using Microsoft.Extensions.WebEncoders;
using Microsoft.AspNetCore.Http.Authentication;
using System.Security.Claims;
using AdminSiteNew.Database;
using AdminSiteNew.Models;

namespace AdminSiteNew
{
    public class Startup
    {
        public static string Token { get; private set; }

        public Startup(IHostingEnvironment env)
        {
            ServerInteraction.Handler.Start();

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }
            Configuration = builder.Build();
            Token = Configuration["Token"];
            Console.WriteLine(Token);
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddMvc(x =>
            {

            });

            services.AddIdentity<User, Role>();
            services.AddAuthentication(o =>
                o.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme);
            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    "Moderator",
                    authBuilder =>
                    {
                        authBuilder.RequireClaim("Moderator", "Allowed");
                    });
                options.AddPolicy(
                    "Developer",
                    authBuilder =>
                    {
                        authBuilder.RequireClaim("Developer", "Allowed");
                    });
                options.AddPolicy(
                    "Administrator",
                    authBuilder =>
                    {
                        authBuilder.RequireClaim("Administrator", "Allowed");
                    });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(LogLevel.Warning);
            //loggerFactory.AddDebug();
            PokemonDatabase.LoadDatabase();

            //app.UseApplicationInsightsRequestTelemetry();

            app.UseDeveloperExceptionPage();
            app.UseBrowserLink();

            app.UseStaticFiles();

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                ExpireTimeSpan = TimeSpan.FromHours(9),
                LoginPath = new PathString("/login"),
                SlidingExpiration = true
            });

            var options = new GoogleOptions
            {
                ClientId = "978297029125-doscbkcd7p9o7lqa4f9cro5nb2fetfth.apps.googleusercontent.com",
                ClientSecret = "XKJMty6DaT0z7b7kBxlLyrn_",
                AccessType = "offline",
                Events = new OAuthEvents()
                {
                    OnRemoteFailure = ctx =>
                    {
                        throw ctx.Failure;
                        ctx.Response.Redirect("/error?ErrorMessage=" +
                                              Microsoft.Extensions.WebEncoders.UrlEncoder.Default.UrlEncode(ctx.Failure
                                                  .Message));
                        ctx.HandleResponse();
                        return Task.FromResult(0);
                    },
                    OnCreatingTicket = x =>
                    {
                        Console.WriteLine(x.ExpiresIn);
                        var permissionLevel = Database.Database.RegisterWebAdmin((string)x.User["id"], x.Identity.Name).Result;
                        if (permissionLevel >= 1)
                        {
                            x.Identity.AddClaim(new Claim("Moderator", "Allowed"));
                        }
                        if (permissionLevel >= 2)
                        {
                            x.Identity.AddClaim(new Claim("Developer", "Allowed"));
                        }
                        if (permissionLevel >= 3)
                        {
                            x.Identity.AddClaim(new Claim("Administrator", "Allowed"));
                        }
                        return Task.FromResult(0);
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
                    context.Response.Redirect("/login?authscheme=Google");
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
