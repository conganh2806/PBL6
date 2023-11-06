using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Webnovel.API.Databases;
using WebNovel.API.Areas.Models.Accounts;
using WebNovel.API.Areas.Models.Bookmarked;
using WebNovel.API.Areas.Models.Chapter;
using WebNovel.API.Areas.Models.Comment;
using WebNovel.API.Areas.Models.Genres;
using WebNovel.API.Areas.Models.Login;
using WebNovel.API.Areas.Models.Novels;
using WebNovel.API.Areas.Models.Preferences;
using WebNovel.API.Areas.Models.Rating;
using WebNovel.API.Areas.Models.Roles;
using WebNovel.API.Areas.Models.UpdatedFees;
using WebNovel.API.Core.Services;
using WebNovel.API.Core.Services.Schemas;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("AzureMySQL");
// Add services to the container.
var services = builder.Services;
services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var serverVersion = ServerVersion.AutoDetect(connectionString);
services.AddDbContext<DataContext>(
    dbContextOptions => dbContextOptions
        .UseMySql(connectionString, serverVersion, options => options.EnableRetryOnFailure())
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors()
);
services.AddScoped<IAccountModel, AccountModel>();
services.AddScoped<IGenreModel, GenreModel>();
services.AddScoped<IRoleModel, RoleModel>();
services.AddScoped<INovelModel, NovelModel>();
services.AddScoped<IChapterModel, ChapterModel>();
services.AddScoped<IPreferencesModel, PreferencesModel>();
services.AddScoped<IRatingModel, RatingModel>();
services.AddScoped<ICommentModel, CommentModel>();
services.AddScoped<IBookmarkedModel, BookmarkedModel>();
services.AddScoped<IUpdatedFeeModel, UpdatedFeeModel>();
services.AddScoped<ILogService, LogService>();
services.AddScoped<IAwsS3Service, AwsS3Service>();
services.AddScoped<ITokenService, TokenService>();
services.AddScoped<ILoginModel, LoginModel>();

services.AddOptions<JwtSettings>()
    .BindConfiguration($"{nameof(JwtSettings)}")
    .ValidateDataAnnotations()
    .ValidateOnStart();

services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration.GetSection("JwtSettings")["Key"]!))
        };
    });

services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API",
        Version = "v1"
    });
    // To Enable authorization using Swagger (JWT)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
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


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
