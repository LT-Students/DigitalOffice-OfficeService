using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using HealthChecks.UI.Client;
using LT.DigitalOffice.Kernel.BrokerSupport.Configurations;
using LT.DigitalOffice.Kernel.BrokerSupport.Extensions;
using LT.DigitalOffice.Kernel.BrokerSupport.Middlewares.Token;
using LT.DigitalOffice.Kernel.Configurations;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Helpers;
using LT.DigitalOffice.Kernel.Middlewares.ApiInformation;
using LT.DigitalOffice.Kernel.RedisSupport.Configurations;
using LT.DigitalOffice.Kernel.RedisSupport.Constants;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers;
using LT.DigitalOffice.OfficeService.Broker.Consumers;
using LT.DigitalOffice.OfficeService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.OfficeService.Models.Dto.Configuration;
using MassTransit;
using MassTransit.RabbitMqTransport;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Serilog;
using StackExchange.Redis;

namespace LT.DigitalOffice.OfficeService
{
  public class Startup : BaseApiInfo
  {
    public const string CorsPolicyName = "LtDoCorsPolicy";
    private string redisConnStr;

    private readonly RabbitMqConfig _rabbitMqConfig;
    private readonly BaseServiceInfoConfig _serviceInfoConfig;

    private IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;

      _rabbitMqConfig = Configuration
        .GetSection(BaseRabbitMqConfig.SectionName)
        .Get<RabbitMqConfig>();

      _serviceInfoConfig = Configuration
        .GetSection(BaseServiceInfoConfig.SectionName)
        .Get<BaseServiceInfoConfig>();

      Version = "1.0.0.0";
      Description = "OfficeService is an API that intended to work with offices.";
      StartTime = DateTime.UtcNow;
      ApiName = $"LT Digital Office - {_serviceInfoConfig.Name}";
    }

    #region public methods

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddCors(options =>
      {
        options.AddPolicy(
          CorsPolicyName,
          builder =>
          {
            builder
              .AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
          });
      });

      if (int.TryParse(Environment.GetEnvironmentVariable("RedisCacheLiveInMinutes"), out int redisCacheLifeTime))
      {
        services.Configure<RedisConfig>(options =>
        {
          options.CacheLiveInMinutes = redisCacheLifeTime;
        });
      }
      else
      {
        services.Configure<RedisConfig>(Configuration.GetSection(RedisConfig.SectionName));
      }

      services.Configure<TokenConfiguration>(Configuration.GetSection("CheckTokenMiddleware"));
      services.Configure<BaseRabbitMqConfig>(Configuration.GetSection(BaseRabbitMqConfig.SectionName));
      services.Configure<BaseServiceInfoConfig>(Configuration.GetSection(BaseServiceInfoConfig.SectionName));

      services.AddHttpContextAccessor();
      services
        .AddControllers()
        .AddJsonOptions(options =>
        {
          options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        })
        .AddNewtonsoftJson();


      string connStr = Environment.GetEnvironmentVariable("ConnectionString");
      if (string.IsNullOrEmpty(connStr))
      {
        connStr = Configuration.GetConnectionString("SQLConnectionString");

        Log.Information($"SQL connection string from appsettings.json was used. Value '{HidePasswordHelper.HidePassword(connStr)}'.");
      }
      else
      {
        Log.Information($"SQL connection string from environment was used. Value '{HidePasswordHelper.HidePassword(connStr)}'.");
      }

      services.AddDbContext<OfficeServiceDbContext>(options =>
      {
        options.UseSqlServer(connStr);
      });

      services.AddHealthChecks()
        .AddSqlServer(connStr)
        .AddRabbitMqCheck();

      redisConnStr = Environment.GetEnvironmentVariable("RedisConnectionString");
      if (string.IsNullOrEmpty(redisConnStr))
      {
        redisConnStr = Configuration.GetConnectionString("Redis");

        Log.Information($"Redis connection string from appsettings.json was used. Value '{HidePasswordHelper.HidePassword(redisConnStr)}'");
      }
      else
      {
        Log.Information($"Redis connection string from environment was used. Value '{HidePasswordHelper.HidePassword(redisConnStr)}'");
      }

      services.AddSingleton<IConnectionMultiplexer>(
        x => ConnectionMultiplexer.Connect(redisConnStr + ",abortConnect=false,connectRetry=1,connectTimeout=2000"));

      services.AddBusinessObjects();

      ConfigureMassTransit(services);
    }

    public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
    {
      UpdateDatabase(app);

      string error = FlushRedisDbHelper.FlushDatabase(redisConnStr, Cache.Offices);
      if (error is not null)
      {
        Log.Error(error);
      }

      app.UseForwardedHeaders();

      app.UseExceptionsHandler(loggerFactory);

      app.UseApiInformation();

      app.UseRouting();

      app.UseMiddleware<TokenMiddleware>();

      app.UseCors(CorsPolicyName);

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers().RequireCors(CorsPolicyName);

        endpoints.MapHealthChecks($"/{_serviceInfoConfig.Id}/hc", new HealthCheckOptions
        {
          ResultStatusCodes = new Dictionary<HealthStatus, int>
          {
            { HealthStatus.Unhealthy, 200 },
            { HealthStatus.Healthy, 200 },
            { HealthStatus.Degraded, 200 },
          },
          Predicate = check => check.Name != "masstransit-bus",
          ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
      });
    }

    #endregion

    #region private methods

    private (string username, string password) GetRabbitMqCredentials()
    {
      static string GetString(string envVar, string formAppsettings, string generated, string fieldName)
      {
        string str = Environment.GetEnvironmentVariable(envVar);
        if (string.IsNullOrEmpty(str))
        {
          str = formAppsettings ?? generated;

          Log.Information(
            formAppsettings == null
              ? $"Default RabbitMq {fieldName} was used."
              : $"RabbitMq {fieldName} from appsetings.json was used.");
        }
        else
        {
          Log.Information($"RabbitMq {fieldName} from environment was used.");
        }

        return str;
      }

      return (GetString("RabbitMqUsername", _rabbitMqConfig.Username, $"{_serviceInfoConfig.Name}_{_serviceInfoConfig.Id}", "Username"),
        GetString("RabbitMqPassword", _rabbitMqConfig.Password, _serviceInfoConfig.Id, "Password"));
    }

    private void ConfigureMassTransit(IServiceCollection services)
    {
      (string username, string password) = GetRabbitMqCredentials();
      services.AddMassTransit(x =>
      {
        x.AddConsumer<CreateUserOfficeConsumer>();
        x.AddConsumer<GetOfficesConsumer>();
        x.AddConsumer<DisactivateOfficeUserConsumer>();
        x.AddConsumer<FilterOfficesUsersConsumer>();

        x.UsingRabbitMq((context, cfg) =>
          {
            cfg.Host(_rabbitMqConfig.Host, "/", host =>
              {
                host.Username(username);
                host.Password(password);
              });

            ConfigureEndpoints(context, cfg);
          });

        x.AddRequestClients(_rabbitMqConfig);
      });

      services.AddMassTransitHostedService();
    }

    private void ConfigureEndpoints(
      IBusRegistrationContext context,
      IRabbitMqBusFactoryConfigurator cfg)
    {
      cfg.ReceiveEndpoint(_rabbitMqConfig.GetOfficesEndpoint, ep =>
      {
        ep.ConfigureConsumer<GetOfficesConsumer>(context);
      });

      cfg.ReceiveEndpoint(_rabbitMqConfig.CreateUserOfficeEndpoint, ep =>
      {
        ep.ConfigureConsumer<CreateUserOfficeConsumer>(context);
      });

      cfg.ReceiveEndpoint(_rabbitMqConfig.DisactivateOfficeUserEndpoint, ep =>
      {
        ep.ConfigureConsumer<DisactivateOfficeUserConsumer>(context);
      });

      cfg.ReceiveEndpoint(_rabbitMqConfig.FilterOfficesEndpoint, ep =>
      {
        ep.ConfigureConsumer<FilterOfficesUsersConsumer>(context);
      });
    }

    private void UpdateDatabase(IApplicationBuilder app)
    {
      using var serviceScope = app.ApplicationServices
        .GetRequiredService<IServiceScopeFactory>()
        .CreateScope();

      using var context = serviceScope.ServiceProvider.GetService<OfficeServiceDbContext>();

      context.Database.Migrate();
    }

    #endregion
  }
}
