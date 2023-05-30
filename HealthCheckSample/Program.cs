using Serilog.Events;
using Serilog;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using FluentValidation.AspNetCore;
using HealthCheckSample.Services.Util;
using HealthCheckSample.Services;
using HealthCheckSample.Models.Validator;
using FluentValidation;
using HealthCheckSample.Controllers;
using UserHealthCheck;
using WeatherHealthCheck;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

#region Logger Configuration
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Log/log.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 31)
    .CreateLogger();
#endregion

try
{
    Log.Information("Starting Web Application");

    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();

    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    string MyAllowOrigins = "AllowAny";
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(MyAllowOrigins, policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });

    //解決:因中文被視為非拉丁字元，避免被進行特殊編碼
    builder.Services.AddSingleton<HtmlEncoder>(HtmlEncoder.Create(allowedRanges: new[] { UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs }));

    //註冊 FluentValidation
    builder.Services.AddFluentValidationAutoValidation();
    //註冊 Validator
    builder.Services.AddValidatorsFromAssemblyContaining<UserCreateValidator>();
    builder.Services.AddValidatorsFromAssemblyContaining<UserUpdateValidator>();

    #region Service
    builder.Services.AddSingleton<ConfigService>();
    builder.Services.AddSingleton<MSSqlDBService>();
    builder.Services.AddSingleton<UserService>();
    //須把要HealthCheck的檔案注入
    builder.Services.AddSingleton<WeatherForecastController>();
    builder.Services.AddSingleton<UserController>();
    #endregion

    #region HealthCheck
    //要檢查的項目有順序之分， 下列是要檢查的[資料庫連線],[API路由網址],[實作自創邏輯的檢查介面]
    builder.Services.AddHealthChecks()
    .AddSqlServer(builder.Configuration.GetConnectionString("User"))//資料庫連線
    .AddUrlGroup(new Uri("https://localhost:7066/User"), name: "UserUrlGroup")//API路由網址
    .AddCheck<UserApiHealthCheck>("user_api")//實作自創邏輯的檢查介面
    .AddUrlGroup(new Uri("https://localhost:7066/WeatherForecast"), name: "WeatherForecastHealthChecks")
    .AddCheck<WeatherForecastHealthCheck>("weather_forecast_api");

    //此為內存結果數據，需要時可提供給HealthChecksUI使用
    builder.Services.AddHealthChecksUI()
        .AddInMemoryStorage();
    #endregion
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    //下列Endpoint端點 需先開啟路由，後面端點才可正確配置
    app.UseRouting();
    app.UseCors();

    #region HealthCheck
    //下列 .MapHealthChecks()內的參數， 必須與appSetting的Uri相互對照。
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapHealthChecks("/healthcheck", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
        });

        endpoints.MapHealthChecks("/user/healthcheck", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
            Predicate = (check) => check.Name == "UserUrlGroup"//此處可指定顯示哪些檢查結果，需與Program.cs上方Build處的命名吻合
        });

        endpoints.MapHealthChecks("/weatherALL/healthcheck", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
            Predicate = (check) => check.Name == "WeatherForecastHealthChecks" || check.Name == "weather_forecast_api"
        });
        endpoints.MapHealthChecks("/weatherAPI/healthcheck", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
            Predicate = (check) => check.Name == "weather_forecast_api"
        });
        endpoints.MapHealthChecks("/weatherURL/healthcheck", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
            Predicate = (check) => check.Name == "WeatherForecastHealthChecks"
        });

        endpoints.MapHealthChecksUI();
    });
    #endregion
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}


