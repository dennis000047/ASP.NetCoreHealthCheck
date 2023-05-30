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

    //�ѨM:�]����Q�����D�ԤB�r���A�קK�Q�i��S��s�X
    builder.Services.AddSingleton<HtmlEncoder>(HtmlEncoder.Create(allowedRanges: new[] { UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs }));

    //���U FluentValidation
    builder.Services.AddFluentValidationAutoValidation();
    //���U Validator
    builder.Services.AddValidatorsFromAssemblyContaining<UserCreateValidator>();
    builder.Services.AddValidatorsFromAssemblyContaining<UserUpdateValidator>();

    #region Service
    builder.Services.AddSingleton<ConfigService>();
    builder.Services.AddSingleton<MSSqlDBService>();
    builder.Services.AddSingleton<UserService>();
    //����nHealthCheck���ɮת`�J
    builder.Services.AddSingleton<WeatherForecastController>();
    builder.Services.AddSingleton<UserController>();
    #endregion

    #region HealthCheck
    //�n�ˬd�����ئ����Ǥ����A �U�C�O�n�ˬd��[��Ʈw�s�u],[API���Ѻ��}],[��@�۳��޿誺�ˬd����]
    builder.Services.AddHealthChecks()
    .AddSqlServer(builder.Configuration.GetConnectionString("User"))//��Ʈw�s�u
    .AddUrlGroup(new Uri("https://localhost:7066/User"), name: "UserUrlGroup")//API���Ѻ��}
    .AddCheck<UserApiHealthCheck>("user_api")//��@�۳��޿誺�ˬd����
    .AddUrlGroup(new Uri("https://localhost:7066/WeatherForecast"), name: "WeatherForecastHealthChecks")
    .AddCheck<WeatherForecastHealthCheck>("weather_forecast_api");

    //�������s���G�ƾڡA�ݭn�ɥi���ѵ�HealthChecksUI�ϥ�
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

    //�U�CEndpoint���I �ݥ��}�Ҹ��ѡA�᭱���I�~�i���T�t�m
    app.UseRouting();
    app.UseCors();

    #region HealthCheck
    //�U�C .MapHealthChecks()�����ѼơA �����PappSetting��Uri�ۤ���ӡC
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapHealthChecks("/healthcheck", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
        });

        endpoints.MapHealthChecks("/user/healthcheck", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
            Predicate = (check) => check.Name == "UserUrlGroup"//���B�i���w��ܭ����ˬd���G�A�ݻPProgram.cs�W��Build�B���R�W�k�X
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


