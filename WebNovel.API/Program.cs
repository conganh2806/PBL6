using Microsoft.EntityFrameworkCore;
using Webnovel.API.Databases;
using WebNovel.API.Areas.Models.Accounts;
using WebNovel.API.Areas.Models.Chapter;
using WebNovel.API.Areas.Models.Genres;
using WebNovel.API.Areas.Models.Novels;
using WebNovel.API.Areas.Models.Preferences;
using WebNovel.API.Areas.Models.Rating;
using WebNovel.API.Areas.Models.Roles;
using WebNovel.API.Areas.Models.UpdatedFees;
using WebNovel.API.Core.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
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
services.AddScoped<IUpdatedFeeModel, UpdatedFeeModel>();
services.AddScoped<ILogService, LogService>();
services.AddScoped<IAwsS3Service, AwsS3Service>();



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
