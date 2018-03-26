using Conduit.Context;
using Conduit.Repositories;
using Conduit.Services;
using Conduit.Validators.AuthorizationHandler;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;

namespace Conduit
{
    public class Startup
    {
        public Startup(ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _logger = loggerFactory.CreateLogger<Startup>();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        private ILogger _logger { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ConduitDbContext>().AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.User.RequireUniqueEmail = true;
            });

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = Configuration["Jwt:Issuer"],
                ValidAudience = Configuration["Jwt:Issuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
            };
            JwtBearerEvents events = new JwtBearerEvents()
            {
                OnMessageReceived = context =>
                {
                    string auth = context.Request.Headers["Authorization"];
                    if (auth?.StartsWith("Token ", StringComparison.OrdinalIgnoreCase) ?? false)
                    {
                        context.Request.Headers["Authorization"] = "Bearer " + auth.Substring("Token ".Length).Trim();
                    }
                    return Task.CompletedTask;
                }
            };

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = tokenValidationParameters;
                    options.Events = events;
                    options.IncludeErrorDetails = true;
                })
                .AddJwtBearer("Token", options =>
                {
                    options.TokenValidationParameters = tokenValidationParameters;
                    options.Events = events;
                    options.IncludeErrorDetails = true;
                });

            services.AddDbContext<ConduitDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("ConduitDB")));

            services.AddMvc();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("ValidUsername", policy =>
                    policy.Requirements.Add(new ValidUsernameRequirement()));
            });

            services.AddScoped<IAuthorizationHandler, ValidJWTAuthorizationHandler>();

            //Custom services
            services.AddScoped<IArticleService, ArticleService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IUserService, UserService>();

            //Custom repositories
            services.AddScoped<IArticleRepository, ArticleRepository>();
            services.AddScoped<IArticleTagsRepository, ArticleTagsRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IFavoriteArticleRepository, FavoriteArticleRepository>();
            services.AddScoped<ITagsRepository, TagsRepository>();
            services.AddScoped<IUserIsFollowingRepository, UserIsFollowingRepository>();
            services.AddScoped<IUserPersonalizationRepository, UserPersonalizationRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
