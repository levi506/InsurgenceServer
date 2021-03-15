using AdminSite.Pokemon;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AdminSite
{
    public class Startup
    {
        public static string Token { get; private set; }

        public Startup(IConfiguration configuration)
        {
            ServerInteraction.Handler.Start();
            PokemonDatabase.LoadDatabase();

            Configuration = configuration;
            Token         = Configuration["Token"];
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(x => { x.EnableEndpointRouting = false; });

            //services.AddIdentity<User, Role>()
            //    .AddDefaultTokenProviders();
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

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.ExpireTimeSpan = TimeSpan.FromHours(12);
                    options.LoginPath = new PathString("/Account/Login");
                    options.SlidingExpiration = true;
                    options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
                })
                .AddGoogle(x =>
                {
                    x.ClientId = Environment.GetEnvironmentVariable("GClientId");
                    x.ClientSecret = Environment.GetEnvironmentVariable("GClientSecret");
                    x.AccessType = "offline";
                    x.Events = new OAuthEvents()
                    {
                        OnRemoteFailure = ctx => throw ctx.Failure,
                        OnCreatingTicket = y =>
                        {
                            var permissionLevel = Database.Database.RegisterWebAdmin(y.User.GetString("id"), y.Identity.Name).Result;
                            if (permissionLevel >= 1)
                            {
                                y.Identity.AddClaim(new Claim("Moderator", "Allowed"));
                            }
                            if (permissionLevel >= 2)
                            {
                                y.Identity.AddClaim(new Claim("Developer", "Allowed"));
                            }
                            if (permissionLevel >= 3)
                            {
                                y.Identity.AddClaim(new Claim("Administrator", "Allowed"));
                            }
                            return Task.FromResult(0);
                        }
                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseAuthentication();


            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            /*
            app.Map("/login", signoutApp =>
            {
                signoutApp.Run(async context =>
                {
                    var authType = context.Request.Query["authscheme"];
                    if (!string.IsNullOrEmpty(authType))
                    {
                        // By default the client will be redirect back to the URL that issued the challenge (/login?authtype=foo),
                        // send them to the home page instead (/).
                        await context.ChallengeAsync(authType, new AuthenticationProperties() { RedirectUri = "/" });
                        return;
                    }
                    context.Response.Redirect("/login?authscheme=Google");
                });
            });*/
            app.Map("/logout", signoutApp =>
            {
                signoutApp.Run(async context =>
                {
                    context.Response.ContentType = "text/html";
                    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    await context.Response.WriteAsync("<html><body>");
                    await context.Response.WriteAsync("You have been logged out. Goodbye " + context.User.Identity.Name + "<br>");
                    await context.Response.WriteAsync("<a href=\"/\">Home</a>");
                    await context.Response.WriteAsync("</body></html>");
                });
            });

        }
    }

    public class User : IdentityUser<string>
    {
    }

    public class Role : IdentityRole<string>
    {

    }
}