using ClinicaWeb.Application.EmailsInfrastructure.Email;
using ClinicaWeb.Application.EmailsInfrastructure;
using ClinicaWeb.Application.ExceptionHandling;
using ClinicaWeb.Application.Services;
using ClinicaWeb.Application.Users.Commands;
using ClinicaWeb.Persistence;
using ClinicaWeb.Shared.Dtos.Pagination.Service;
using ClinicaWeb.Shared.Enums;
using MediatR.Pipeline;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using ClinicaWeb.Application.Users;
using ClinicaWeb.Persistence.Initializer;
using Microsoft.OpenApi.Models;

namespace ClinicaWeb.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
            });

            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly(), typeof(UserProfile).Assembly);

            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CreateUserCommand>());

            // REGISTER SERVER SERVICES
            builder.Services.AddSingleton<IPaginationService, PaginationService>();

            builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("Smtp"));

            builder.Services.AddTransient<IEmailService, EmailService>();
            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddCookie(x =>
                {
                    x.Cookie.Name = "token";
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(builder.Configuration.GetSection("AppSettings:Token").Value)),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        RequireExpirationTime = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                        ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha512 } // Specify the expected signing algorithm
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            context.Token = context.Request.Cookies["token"];
                            return Task.CompletedTask;
                        }
                    };
                });

            builder.Services.AddAuthorizationCore(options =>
            {
                options.AddPolicy("SuperAdmin", policy => policy.RequireClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", Role.Administrator.ToString()));
                options.AddPolicy("ChallengeCreator", policy => policy.RequireClaim("role", Role.PracticianInjectari.ToString()));
                options.AddPolicy("ChallengeReviewer", policy => policy.RequireClaim("role", Role.PracticianLaser.ToString()));
                options.AddPolicy("CtfAdmin", policy => policy.RequireClaim("role", Role.PracticianCuratari.ToString()));
                options.AddPolicy("Member", policy => policy.RequireClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", Role.Client.ToString()));
                options.AddPolicy("SuperAdminAndMember", policy => policy.RequireClaim("role", Role.Administrator.ToString(), Role.Client.ToString()));
            });


            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "CODWER ACADEMY API",
                    Version = "v1"
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }

                });
            });

            builder.Services.AddControllers(o =>
            {
                o.Filters.Add<CustomExceptionFilter>();
                //o.Filters.Add<AuthorizationExceptionFilter>();
            });

            var app = builder.Build();

            using (var scope = builder.Services.BuildServiceProvider().CreateScope())
            {
                using (var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>())
                {
                    if (dbContext.Database.GetPendingMigrations().Any())
                    {
                        dbContext.Database.Migrate();
                    }
                    DbSeeder.SeedDb(dbContext);
                }
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwaggerUI();
                app.UseSwagger();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();

            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();
            app.MapControllers();
            app.MapFallbackToFile("index.html");

            app.Run();
        }
    }
}
