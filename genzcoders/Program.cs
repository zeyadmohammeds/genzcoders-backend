using genzcoders.Data;
using genzcoders.Models;
using genzcoders.Repos.ApplicationRepo;
using genzcoders.Repos.CourseRepo;
using genzcoders.Repos.RoundRepo;
using genzcoders.Repos.MaterialRepo;
using genzcoders.Repos.ZoomMeetingRepo;
using genzcoders.Repos.CourseRoundStudentRepo;
using genzcoders.Repos.WeekRepo;
using genzcoders.Services.ApplicationService;
using genzcoders.Services.CourseService;
using genzcoders.Services.RoundService;
using genzcoders.Services.MaterialService;
using genzcoders.Services.ZoomMeetingService;
using genzcoders.Services.CourseRoundStudentService;
using genzcoders.Services.WeekService;
using genzcoders.Services.EmailService;
using genzcoders.Services.OtpService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace genzcoders
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.WriteIndented = true;
                });
            
            // Add Swagger/OpenAPI services
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    sql =>
                    {
                        sql.EnableRetryOnFailure();
                        sql.CommandTimeout(60);
                    }));

            builder.Services.AddScoped<ICourseRepo, CourseRepo>();
            builder.Services.AddScoped<ICourseService, CourseService>();
            builder.Services.AddScoped<IRoundRepo, RoundRepo>();
            builder.Services.AddScoped<IRoundService, RoundService>();
            builder.Services.AddScoped<IApplicationRepo, ApplicationRepo>();
            builder.Services.AddScoped<IApplicationService, ApplicationService>();
            builder.Services.AddScoped<IMaterialRepo, MaterialRepo>();
            builder.Services.AddScoped<IMaterialService, MaterialService>();
            builder.Services.AddScoped<IZoomMeetingRepo, ZoomMeetingRepo>();
            builder.Services.AddScoped<IZoomMeetingService, ZoomMeetingService>();
            builder.Services.AddScoped<ICourseRoundStudentRepo, CourseRoundStudentRepo>();
            builder.Services.AddScoped<ICourseRoundStudentService, CourseRoundStudentService>();
            builder.Services.AddScoped<IWeekRepo, WeekRepo>();
            builder.Services.AddScoped<IWeekService, WeekService>();

            // Email and OTP services
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IOtpService, OtpService>();

            // Password hasher
            builder.Services.AddScoped<IPasswordHasher<Account>, PasswordHasher<Account>>();
            
            // Rate limiting for security
            builder.Services.AddMemoryCache();

            var jwtKey = builder.Configuration["Jwt:Key"];
            var jwtIssuer = builder.Configuration["Jwt:Issuer"];
            var jwtAudience = builder.Configuration["Jwt:Audience"];

            if (string.IsNullOrWhiteSpace(jwtKey))
            {
                throw new InvalidOperationException("Missing JWT signing key. Set configuration value Jwt:Key.");
            }

            // Configure Google OAuth
            var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
            var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                        ValidateIssuer = !string.IsNullOrWhiteSpace(jwtIssuer),
                        ValidIssuer = jwtIssuer,
                        ValidateAudience = !string.IsNullOrWhiteSpace(jwtAudience),
                        ValidAudience = jwtAudience,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(1)
                    };
                })
                .AddGoogle(options =>
                {
                    if (!string.IsNullOrWhiteSpace(googleClientId) && !string.IsNullOrWhiteSpace(googleClientSecret))
                    {
                        options.ClientId = googleClientId;
                        options.ClientSecret = googleClientSecret;
                    }
                });

            // Add Authorization with role-based policies
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireAssertion(context =>
                    context.User.HasClaim("roleId", GetAdminRoleId(builder.Configuration).ToString())));
                
                options.AddPolicy("InstructorOnly", policy => policy.RequireAssertion(context =>
                    context.User.HasClaim("roleId", GetInstructorRoleId(builder.Configuration).ToString())));
                
                options.AddPolicy("AdminOrInstructor", policy => policy.RequireAssertion(context =>
                {
                    var roleId = context.User.FindFirst("roleId")?.Value;
                    var adminRoleId = GetAdminRoleId(builder.Configuration);
                    var instructorRoleId = GetInstructorRoleId(builder.Configuration);
                    return roleId == adminRoleId.ToString() || roleId == instructorRoleId.ToString();
                }));
            });

            // Add CORS - Configure for frontend connection
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
                    
                    if (allowedOrigins != null && allowedOrigins.Length > 0)
                    {
                        // Use specific origins from configuration
                        policy.WithOrigins(allowedOrigins)
                              .AllowAnyMethod()
                              .AllowAnyHeader()
                              .AllowCredentials();
                    }
                    else
                    {
                        // Fallback: Allow all origins (for development)
                        policy.AllowAnyOrigin()
                              .AllowAnyMethod()
                              .AllowAnyHeader();
                    }
                });
            });

            var app = builder.Build();

            var forwardedHeadersOptions = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            };
            forwardedHeadersOptions.KnownNetworks.Clear();
            forwardedHeadersOptions.KnownProxies.Clear();
            app.UseForwardedHeaders(forwardedHeadersOptions);

            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
                    app.Logger.LogError(exception, "Unhandled exception");

                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsJsonAsync(new
                    {
                        message = "An unexpected error occurred."
                    });
                });
            });

            // Configure Swagger UI - available in all environments
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "GenzCoders API v1");
                options.RoutePrefix = "swagger";
            });

            app.UseHttpsRedirection();
            
            // Add security headers
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("X-Frame-Options", "DENY");
                context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
                context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
                
                // Don't expose server information
                context.Response.Headers.Remove("Server");
                
                await next();
            });
            
            app.UseRouting();

            // Use CORS - must be after UseRouting and before UseAuthentication
            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }

        private static long GetAdminRoleId(IConfiguration configuration)
        {
            // This will be resolved from database at runtime in controllers
            // For now, return a placeholder - actual implementation uses database lookup
            return 1;
        }

        private static long GetInstructorRoleId(IConfiguration configuration)
        {
            // This will be resolved from database at runtime in controllers
            // For now, return a placeholder - actual implementation uses database lookup
            return 2;
        }
    }
}
