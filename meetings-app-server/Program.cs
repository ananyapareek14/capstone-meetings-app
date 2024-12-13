using meetings_app_server.Repositories;
using meetings_app_server.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using meetings_app_server.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure DbContext to use SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IMeetingRepository, MeetingRepository>();

// Register JwtService
builder.Services.AddSingleton<JwtService>();

// Configure authentication with JWT (without hardcoding the key)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "localhost:5000",
            ValidAudience = "localhost:5000"
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var userId = context.Principal?.FindFirst("userId")?.Value;
                if (userId == null)
                {
                    context.Fail("Unauthorized");
                    return;
                }

                var userRepository = context.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
                var dynamicKey = await userRepository.GetUserSessionKey(Guid.Parse(userId));

                if (dynamicKey == null)
                {
                    context.Fail("Unauthorized");
                    return;
                }

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(dynamicKey));
                context.Options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = securityKey
                };
            }
        };
    });

// Enable Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // Ensure authentication middleware is added
app.UseAuthorization();

app.MapControllers();

// Migrate the database on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

app.Run();