using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using YouNGU.Data;
using YouNGU.Data.Repositories;
using YouNGU.Models.Entities;
using YouNGU.Repositories;
using YouNGU.Services;
using YouNGU.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Chỉ giữ lại một cách đăng ký Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.MSSqlServer(
        connectionString: builder.Configuration.GetConnectionString("DefaultConnection"),
        sinkOptions: new Serilog.Sinks.MSSqlServer.MSSqlServerSinkOptions
        {
            TableName = "Logs",
            AutoCreateSqlTable = true
        },
        columnOptions: new Serilog.Sinks.MSSqlServer.ColumnOptions
        {
            AdditionalColumns = new List<Serilog.Sinks.MSSqlServer.SqlColumn>
            {
                new Serilog.Sinks.MSSqlServer.SqlColumn
                {
                    ColumnName = "SourceContext",
                    DataType = System.Data.SqlDbType.NVarChar,
                    DataLength = 255
                },
                new Serilog.Sinks.MSSqlServer.SqlColumn
                {
                    ColumnName = "RequestPath",
                    DataType = System.Data.SqlDbType.NVarChar,
                    DataLength = 255
                },
                new Serilog.Sinks.MSSqlServer.SqlColumn
                {
                    ColumnName = "RequestMethod",
                    DataType = System.Data.SqlDbType.NVarChar,
                    DataLength = 50
                },
                new Serilog.Sinks.MSSqlServer.SqlColumn
                {
                    ColumnName = "ActionName",
                    DataType = System.Data.SqlDbType.NVarChar,
                    DataLength = 50
                }
            }
        }
    ));

builder.Services.AddControllersWithViews();

// Đăng ký repositories
builder.Services.AddScoped<IVideoRepository, VideoRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<ILikeRepository, LikeRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
builder.Services.AddScoped<IPlaylistRepository, PlaylistRepository>();
builder.Services.AddScoped<ISettingRepository, SettingRepository>();
builder.Services.AddScoped<ILogRepository, LogRepository>();

// Đăng ký services
builder.Services.AddScoped<IVideoService, VideoService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<SettingService>();
builder.Services.AddScoped<CloudinaryService>();
builder.Services.AddScoped<VideoAnalyticsService>();
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
// Đăng ký SignalR
builder.Services.AddSignalR();




// Cấu hình cookie (bỏ comment nếu cần)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

// Cấu hình Kestrel để tăng giới hạn kích thước request body
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 524_288_000; // 500 MB

    // Bạn cũng có thể đặt các giới hạn khác nếu cần
    serverOptions.Limits.MaxRequestLineSize = 8192; // Giới hạn dòng request
    serverOptions.Limits.MaxRequestHeadersTotalSize = 32768; // Giới hạn tổng kích thước header
});

var app = builder.Build();
// Đăng ký middleware 
app.UseMiddleware<LoggingMiddleware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.MapHub<NotificationHub>("/notificationHub");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
/*
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);
*/
app.MapAreaControllerRoute(
    name: "MyAreaAdmin",
    areaName: "Admin",
    pattern: "Admin/{controller=Dashboard}/{action=Index}/{id?}");
app.MapRazorPages();

// Middleware để ghi log HTTP request
app.UseSerilogRequestLogging(options =>
{
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("RequestPath", httpContext.Request.Path);
        diagnosticContext.Set("RequestMethod", httpContext.Request.Method);
        diagnosticContext.Set("ActionName", httpContext.GetEndpoint()?.DisplayName);
    };
});

// Kh?i t?o d? li?u m?u
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        await SeedData.InitializeAsync(context, userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "L?i khi kh?i t?o d? li?u.");
    }
}
app.Use(async (context, next) =>
{
    Console.WriteLine($"Request Path: {context.Request.Path}");
    await next();
});

app.Run();
