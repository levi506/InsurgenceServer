﻿using System;
using System.Collections.Generic;
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

namespace AdminSiteNew
{
    public class Startup
    {
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
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddMvc();

            services.AddIdentity<User, Role>();
            services.AddAuthentication(o =>
                o.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme);
            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    "Access",
                    authBuilder =>
                    {
                        authBuilder.RequireClaim("Access", "Allowed");
                    });
                options.AddPolicy(
                    "Deuk",
                    authBuilder =>
                    {
                        authBuilder.RequireClaim("Deuk", "Allowed");
                    });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug();
            

            //app.UseApplicationInsightsRequestTelemetry();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }


            app.UseStaticFiles();


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
                    OnRemoteFailure = ctx =>
                    {
                        ctx.Response.Redirect("/error?ErrorMessage=" + Microsoft.Extensions.WebEncoders.UrlEncoder.Default.UrlEncode(ctx.Failure.Message));
                        ctx.HandleResponse();
                        return Task.FromResult(0);
                    },
                    OnCreatingTicket = x =>
                    {
                        if (DatabaseSpace.Database.RegisterWebAdmin((string)x.User["id"], x.Identity.Name))
                        {
                            x.Identity.AddClaim(new Claim("Access", "Allowed"));
                            if ((string)x.User["id"] == "117811387166947407528")
                            {
                                x.Identity.AddClaim(new Claim("Deuk", "Allowed"));
                            }
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