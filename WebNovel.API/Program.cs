using System.Text;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.MySql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Webnovel.API.Databases;
using WebNovel.API.Areas.Models.Accounts;
using WebNovel.API.Areas.Models.Bookmarked;
using WebNovel.API.Areas.Models.Bundles;
using WebNovel.API.Areas.Models.Chapter;
using WebNovel.API.Areas.Models.Comment;
using WebNovel.API.Areas.Models.Genres;
using WebNovel.API.Areas.Models.Login;
using WebNovel.API.Areas.Models.Login.Schemas;
using WebNovel.API.Areas.Models.Merchant;
using WebNovel.API.Areas.Models.Novels;
using WebNovel.API.Areas.Models.Orders;
using WebNovel.API.Areas.Models.Payments;
using WebNovel.API.Areas.Models.Preferences;
using WebNovel.API.Areas.Models.Rating;
using WebNovel.API.Areas.Models.Roles;
using WebNovel.API.Areas.Models.UpdatedFees;
using WebNovel.API.Core.Services;
using WebNovel.API.Core.Services.Schemas;
using WebNovel.API.Core.Services.VnPay.Schemas;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var connectionString = builder.Configuration.GetConnectionString("AzureMySQL");
        // Add services to the container.
        var services = builder.Services;
        services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddHttpContextAccessor();
        services.AddCors(options =>
        {
            options.AddPolicy(name: "WebNovel",
                builder =>
                {
                    builder.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
        });

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
        services.AddScoped<IBundleModel, BundleModel>();
        services.AddScoped<IOrderModel, OrderModel>();
        services.AddScoped<IMerchantModel, MerchantModel>();
        services.AddScoped<IPaymentModel, PaymentModel>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IJobService, JobService>();
        services.AddScoped<IEmailService, SmtpMailService>();

        services.Configure<VnpayConfig>(builder.Configuration.GetSection(VnpayConfig.ConfigName));

        services.AddOptions<JwtSettings>()
            .BindConfiguration($"{nameof(JwtSettings)}")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<GoogleOAuthSettings>()
            .BindConfiguration($"Authentication:{nameof(GoogleOAuthSettings)}");

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

        services.AddHangfire(cfg => cfg
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseStorage(
                new MySqlStorage(
                    builder.Configuration.GetConnectionString("AzureMySQL"),
                    new MySqlStorageOptions
                    {
                        QueuePollInterval = TimeSpan.FromSeconds(10),
                        JobExpirationCheckInterval = TimeSpan.FromHours(1),
                        CountersAggregateInterval = TimeSpan.FromMinutes(5),
                        PrepareSchemaIfNecessary = true,
                        DashboardJobListLimit = 25000,
                        TransactionTimeout = TimeSpan.FromMinutes(1),
                        TablesPrefix = "Hangfire",
                    }
                )
            ));

        services.AddHangfireServer();

        services.AddOptions<MailSettings>()
            .BindConfiguration($"{nameof(MailSettings)}");

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

        app.UseCors("WebNovel");

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        app.UseHangfireDashboard("/hangfire");

        app.Run();
    }
}