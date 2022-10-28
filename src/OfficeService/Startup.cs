using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using DigitalOffice.Kernel.Behaviours;
using DigitalOffice.Kernel.RedisSupport.Extensions;
using FluentValidation;
using HealthChecks.UI.Client;
using LT.DigitalOffice.Kernel.BrokerSupport.Configurations;
using LT.DigitalOffice.Kernel.BrokerSupport.Extensions;
using LT.DigitalOffice.Kernel.BrokerSupport.Middlewares.Token;
using LT.DigitalOffice.Kernel.Configurations;
using LT.DigitalOffice.Kernel.EFSupport.Extensions;
using LT.DigitalOffice.Kernel.EFSupport.Helpers;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Middlewares.ApiInformation;
using LT.DigitalOffice.Kernel.RedisSupport.Configurations;
using LT.DigitalOffice.Kernel.RedisSupport.Constants;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers;
using LT.DigitalOffice.OfficeService.Broker.Consumers;
using LT.DigitalOffice.OfficeService.Business;
using LT.DigitalOffice.OfficeService.DataLayer;
using MassTransit;
using MassTransit.RabbitMqTransport;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Serilog;

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

      Version = "1.0.3.2";
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

      services.AddMediatR(typeof(AssemblyMarker));

      services.AddHttpContextAccessor();
      services
        .AddControllers()
        .AddJsonOptions(options =>
        {
          options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        })
        .AddNewtonsoftJson();

      string connStr = ConnectionStringHandler.Get(Configuration);

      services.AddDbContext<OfficeServiceDbContext>(options =>
      {
        options.UseSqlServer(connStr);
      });

      services.AddHealthChecks()
        .AddSqlServer(connStr)
        .AddRabbitMqCheck();

      redisConnStr = services.AddRedisSingleton(Configuration);

      services.AddBusinessObjects();

      ConfigureMassTransit(services);

      services.AddValidatorsFromAssembly(typeof(AssemblyMarker).Assembly);
      services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehaviour<,>));
    }

    public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
    {
      app.UpdateDatabase<OfficeServiceDbContext>();

      FlushRedisDbHelper.FlushDatabase(redisConnStr, Cache.Offices);

      app.UseForwardedHeaders();

      app.UseExceptionsHandler(loggerFactory);

      app.UseApiInformation();

      app.UseRouting();

      app.UseMiddleware<TokenMiddleware>();

      app.UseCors(CorsPolicyName);

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers().RequireCors(CorsPolicyName);

        endpoints.MapHealthChecks("/hc", new HealthCheckOptions
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
        x.AddConsumer<CheckWorkspaceIsBookableConsumer>();

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

      cfg.ReceiveEndpoint(_rabbitMqConfig.CheckWorkspaceIsBookableEndpoint, ep =>
      {
        ep.ConfigureConsumer<CheckWorkspaceIsBookableConsumer>(context);
      });
    }

    #endregion
  }
}
