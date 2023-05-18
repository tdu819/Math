using System.Text;
using Entities;
using Math.BLL;
using Math.DAL;
using Math.DAL.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Math.WEB;

internal class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add secrets configuration
        builder.Configuration.AddUserSecrets<Program>();

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        // Configure DbContext with the retrieved connection string

        builder.Services.AddDbContext<MathContext>(
            options => options.UseSqlServer(connectionString));

        builder.Services.InstallRepositories();
        builder.Services.InstallMappers();
        builder.Services.InstallServices();

        
        builder.Services.AddAuthentication();
        builder.Services.AddIdentityCore<ApplicationUser>()
            .AddEntityFrameworkStores<MathContext>()
            .AddSignInManager<SignInManager<ApplicationUser>>()
            .AddDefaultTokenProviders();

        builder.Services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 4;
        });

        var key = Encoding.UTF8.GetBytes(builder.Configuration["ApplicationSettings:JWT_Secret"]);

        builder.Services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = false;
            x.SaveToken = false;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };
        });


        builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

        builder.Services.AddCors();

        builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddDbContext<MathContext>();
        

        var app = builder.Build();

// Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        var client_url = builder.Configuration["ApplicationSettings:Client_URL"];
        
        app.UseCors(b => b.WithOrigins(client_url)
            .AllowAnyHeader()
            .AllowAnyMethod());

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
