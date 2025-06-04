using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProfileService.Data;
using System.Text;

namespace ProfileService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost.UseUrls("http://0.0.0.0:80");

            builder.Services.AddDbContext<ProfileDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false; // Убедись, что это `false`, если ты работаешь без HTTPS
                    options.Authority = "http://auth-service"; // Адрес твоего AuthService
                    options.Audience = "https://kiberone.duckdns.org"; // Убедись, что в токене указывается этот audience
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = "https://auth-service",
                        ValidAudience = "https://kiberone.duckdns.org",
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                    };
                });


            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()     // Разрешаем все домены (в продакшене лучше указывать конкретный)
                          .AllowAnyMethod()     // Разрешаем все методы (GET, POST, PUT, DELETE и т. д.)
                          .AllowAnyHeader();    // Разрешаем все заголовки (Authorization, Content-Type и т. д.)
                });
            });

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ProfileDbContext>();
                db.Database.Migrate();
            }

            app.UseCors("AllowAll");


            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.MapGet("/api/student/health", () => Results.Ok(new { status = "OK" }));

            app.Run();
        }
    }
}
