using System;
using System.Reflection;
using System.Text;

using GozemApi.Models;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using Pluralize.NET;

using Raven.Client.Documents;
using Raven.DependencyInjection;
using Raven.Identity;

namespace GozemApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.Configure<RavenSettings>(Configuration.GetSection("RavenSettings"));
            services.AddRavenDbDocStore(options =>
            {
                options.SectionName = "RavenSettings";
                options.BeforeInitializeDocStore = store => { store.Conventions.IdentityPartsSeparator = '~'; };
            });
            services.AddTransient(provider =>
            {
                var store = provider.GetRequiredService<IDocumentStore>();
                return store.OpenAsyncSession();
            });
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddSingleton<DatabaseAttachmentFileProvider>();
            services.AddSingleton(provider =>
            {
                var options = new FileServerOptions();
                Configuration.GetSection("FileServerOptions").Bind(options);
                return options;
            });
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "GozemApi", Version = "v1"}); });
            services.AddIdentity<ApplicationUser, Raven.Identity.IdentityRole>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequiredLength = 6;
                    options.Password.RequiredUniqueChars = 0;
                    options.Password.RequireUppercase = false;
                    options.User.RequireUniqueEmail = false;
                })
                .AddRavenDbIdentityStores<ApplicationUser, Raven.Identity.IdentityRole>();
            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
            });

            var settings = new JwtSettings();
            Configuration.GetSection("JwtSettings").Bind(settings);
            services.Configure<JwtSettings>(Configuration.GetSection("JwtSettings"));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Audience = settings.ValidAudience;
                    options.ClaimsIssuer = settings.ValidIssuer;
                    options.RequireHttpsMetadata = true;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = settings.ValidIssuer,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(settings.SigningKey)),
                        ValidAudience = settings.ValidAudience,
                        ValidateIssuer = settings.ValidateIssuer,
                        ValidateIssuerSigningKey = settings.ValidateIssuerSigningKey,
                        ValidateLifetime = settings.ValidateLifeTime,
                        ValidateAudience = settings.ValidateAudience,
                        ClockSkew = TimeSpan.FromSeconds(0)
                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "GozemApi v1"));
            }

            var options = app.ApplicationServices.GetRequiredService<FileServerOptions>();
            options.FileProvider = app.ApplicationServices.GetRequiredService<DatabaseAttachmentFileProvider>();

            app.UseHttpsRedirection();

            app.UseFileServer(options);

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}