using Delopro.Bll;
using Delopro.Bll.Interfaces;
using Delopro.Bll.Services;
using Delopro.Data;
using Delopro.Data.Entities;
using Delopro.Data.Interfaces;
using Delopro.Data.Repositories;
using Delopro.Server.Configurations;
using Delopro.Server.Middleware;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;

using System.Text.Json.Serialization;

using static Google.Apis.Drive.v3.DriveService;

var builder = WebApplication.CreateBuilder(args);

const string dataProtectionKeys = "Data Protection Keys";

Directory.CreateDirectory(dataProtectionKeys);

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionKeys))
    .SetApplicationName("Delopro");

var usePostgres = false;

ConfigurationHelper.Initialize(builder.Configuration, builder.Environment.WebRootPath, builder.Environment.EnvironmentName);

builder.Services.AddLogging(logs =>
{
    logs.AddConsole();
});

builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddDistributedMemoryCache();

builder.Services.AddAuthentication(options =>
{
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultForbidScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.Cookie.Name = "Delopro_Cookies";
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.SlidingExpiration = true;
        options.Events.OnRedirectToLogin = (context) =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = context => 
        { 
            context.Response.StatusCode = StatusCodes.Status403Forbidden; 
            return Task.CompletedTask; 
        };
    });

builder.Services.AddAuthorization();

var origins = builder.Configuration.GetSection("CorsOrigins")?.AsEnumerable()?.Select(x => x.Value ?? string.Empty).ToArray();

builder.Services.AddCors(options => options.AddPolicy("AllowClient",
    new CorsPolicyBuilder().WithOrigins(origins ?? [])
    .AllowAnyHeader().AllowAnyMethod().AllowCredentials().Build()));

var connectionString = builder.Configuration.GetConnectionString("MssqlDeloproDb");

if (connectionString == null)
{
    usePostgres = true;
    connectionString = builder.Configuration.GetConnectionString("PostgresDeloproDb");
    builder.Services.AddDbContext<PostgresDeloproDbContext>(optionsBuilder => optionsBuilder.UseNpgsql(connectionString));
    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
}
else
{
    builder.Services.AddDbContext<MssqlDeloproDbContext>(optionsBuilder => optionsBuilder.UseSqlServer(connectionString));
}

if (!usePostgres)
{
    builder.Services.AddScoped<IRepository<User>, UserRepository>(ConfigureRepository<MssqlDeloproDbContext, UserRepository>);
    builder.Services.AddScoped<IRepository<Role>, RoleRepository>(ConfigureRepository<MssqlDeloproDbContext, RoleRepository>);
    builder.Services.AddScoped<IRepository<Chapter>, ChapterRepository>(ConfigureRepository<MssqlDeloproDbContext, ChapterRepository>);
    builder.Services.AddScoped<IRepository<Theme>, ThemeRepository>(ConfigureRepository<MssqlDeloproDbContext, ThemeRepository>);
    builder.Services.AddScoped<IRepository<Captcha>, CaptchaRepository>(ConfigureRepository<MssqlDeloproDbContext, CaptchaRepository>);
    builder.Services.AddScoped<IRepository<Message>, MessageRepository>(ConfigureRepository<MssqlDeloproDbContext, MessageRepository>);
    builder.Services.AddScoped<IRepository<UserRole>, UserRoleRepository>(ConfigureRepository<MssqlDeloproDbContext, UserRoleRepository>);
    builder.Services.AddScoped<IRepository<Visit>, VisitRepository>(ConfigureRepository<MssqlDeloproDbContext, VisitRepository>);
    builder.Services.AddScoped<IRepository<Visitor>, VisitorRepository>(ConfigureRepository<MssqlDeloproDbContext, VisitorRepository>);
    builder.Services.AddScoped<IRepository<Comment>, CommentRepository>(ConfigureRepository<MssqlDeloproDbContext, CommentRepository>);
}
else
{
    builder.Services.AddScoped<IRepository<User>, UserRepository>(ConfigureRepository<PostgresDeloproDbContext, UserRepository>);
    builder.Services.AddScoped<IRepository<Role>, RoleRepository>(ConfigureRepository<PostgresDeloproDbContext, RoleRepository>);
    builder.Services.AddScoped<IRepository<Chapter>, ChapterRepository>(ConfigureRepository<PostgresDeloproDbContext, ChapterRepository>);
    builder.Services.AddScoped<IRepository<Theme>, ThemeRepository>(ConfigureRepository<PostgresDeloproDbContext, ThemeRepository>);
    builder.Services.AddScoped<IRepository<Captcha>, CaptchaRepository>(ConfigureRepository<PostgresDeloproDbContext, CaptchaRepository>);
    builder.Services.AddScoped<IRepository<Message>, MessageRepository>(ConfigureRepository<PostgresDeloproDbContext, MessageRepository>);
    builder.Services.AddScoped<IRepository<UserRole>, UserRoleRepository>(ConfigureRepository<PostgresDeloproDbContext, UserRoleRepository>);
    builder.Services.AddScoped<IRepository<Visit>, VisitRepository>(ConfigureRepository<PostgresDeloproDbContext, VisitRepository>);
    builder.Services.AddScoped<IRepository<Visitor>, VisitorRepository>(ConfigureRepository<PostgresDeloproDbContext, VisitorRepository>);
    builder.Services.AddScoped<IRepository<Comment>, CommentRepository>(ConfigureRepository<PostgresDeloproDbContext, CommentRepository>);
}

if (builder.Environment.IsDevelopment() || usePostgres)
{
    builder.Services.AddScoped<IEmailSender, AzureEmailSender>();
}

if (builder.Environment.IsProduction() && !usePostgres)
{
    builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
}

builder.Services.AddSingleton<CryptoService>();
builder.Services.AddScoped<UserManager>();
builder.Services.AddMemoryCache();

if (builder.Environment.IsProduction())
{
    builder.Services.AddSingleton<DriveService>(provider =>
    {
        var cryptoService = provider.GetService<CryptoService>();
        var secrets = builder.Configuration["GoogleDrive:Secrets"];
        var decryptedContent = cryptoService?.Decrypt(secrets);
        var credential = GoogleCredential.FromJson(decryptedContent);

        if (credential.IsCreateScopedRequired)
        {
            credential = credential.CreateScoped(ScopeConstants.DriveFile);
        }

        var driveService = new DriveService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = builder.Configuration["GoogleDrive:ApplicationName"] ?? string.Empty
        });

        return driveService;
    });

    builder.Services.AddSingleton<IDriveService, GoogleDriveService>();
}

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSingleton<IDriveService, LocalDriveService>();
}

builder.Services.ConfigureAutomapper();

var provider = builder?.Services?.BuildServiceProvider();
using var scope = provider?.CreateScope();
MigrateDatabase(scope);
UploadDocuments(scope);

if (ConfigurationHelper.AvatarsPath is not null && !Directory.Exists(ConfigurationHelper.AvatarsPath))
{
    Directory.CreateDirectory(ConfigurationHelper.AvatarsPath);
}

if (ConfigurationHelper.ChapterImagesPath is not null && !Directory.Exists(ConfigurationHelper.ChapterImagesPath))
{
    Directory.CreateDirectory(ConfigurationHelper.ChapterImagesPath);
}

if (ConfigurationHelper.IconsPath is not null && !Directory.Exists(ConfigurationHelper.IconsPath))
{
    Directory.CreateDirectory(ConfigurationHelper.IconsPath);
}

var app = builder!.Build();

app.UseStatusCodePagesWithReExecute("/home/api/error/{0}");
app.UseMiddleware<ExceptionMiddleware>();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("AllowClient");
app.UseCookiePolicy(
    new CookiePolicyOptions
    {
        Secure = CookieSecurePolicy.Always
    });

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

if (app.Environment.IsProduction())
{
    app.MapWhen(httpContext =>
    {
        var path = httpContext.Request.Path.Value;

        if (path == null)
        {
            return false;
        }

        return !path.StartsWith("/api");
    },
    appBuilder =>
    {
        appBuilder.UseRouting();
        appBuilder.UseAuthorization();

        appBuilder.UseEndpoints(endpoints =>
        {
            endpoints.MapFallbackToFile("index.html");
        });
    });
}

app.Run();

TRepository ConfigureRepository<TDbContext, TRepository>(IServiceProvider provider) where TDbContext : DeloproDbContext where TRepository : class
{
    return Activator.CreateInstance(typeof(TRepository), provider.GetRequiredService<TDbContext>()) as TRepository ?? throw new NullReferenceException();
}

void MigrateDatabase(IServiceScope? scope)
{
    DeloproDbContext? dbContext = null;

    if (!usePostgres)
    {
        dbContext = scope?.ServiceProvider.GetRequiredService<MssqlDeloproDbContext>();
    }
    else
    {
        dbContext = scope?.ServiceProvider.GetRequiredService<PostgresDeloproDbContext>();     
    }

    if (dbContext != null)
    {
        try
        { 
            dbContext?.Database.Migrate();
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}

void UploadDocuments(IServiceScope? scope)
{
    var driveService = scope?.ServiceProvider.GetRequiredService<IDriveService>();
    Task.Run(() => driveService?.RestoreAllDocuments());
}
