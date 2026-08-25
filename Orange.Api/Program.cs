using System;
using System.IO;
using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi;
using Orange.Api.Constraints;
using Orange.Api.Interfaces;
using Orange.Api.Services;
using Orange.Api.utils;


namespace Orange.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddServiceDefaults();
        builder.AddSeqEndpoint(connectionName: "seq");

        builder.AddNpgsqlDbContext<ApplicationDbContext>(connectionName: "OrangeDb");

        IConfigurationRoot configuration;

        if (builder.Environment.IsDevelopment())
        {
            configuration = builder.Configuration.AddJsonFile(
                    "appsettings.Development.json",
                    optional: false,
                    reloadOnChange: true)
                .Build();
        }
        else
        {
            configuration = builder.Configuration.AddJsonFile(
                    "appsettings.json",
                    optional: false,
                    reloadOnChange: true)
                .Build();
        }


        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: "allowedOrigins",
                policy =>
            {
                policy
                    .WithOrigins($"{configuration["Api:BaseUrl"]}")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        builder.Services.AddScoped<IGuildService, GuildService>();


        // Set up versioning for Swagger
        builder.Services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            }

        ).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV"; // format: 'v'major[.minor][.patch]
            options.SubstituteApiVersionInUrl = true;
        }).AddMvc();



        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Orange API",
                Description = "The API for the Orange Discord Verification Bot.",
                Contact = new OpenApiContact
                {
                    Name = "Waylo - Discord Bot Developer",
                    Url = new Uri("https://waylo.tech"),
                    Email = "Waylo@waylo.tech"
                }
            });
            string xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });

        //register routeoptions for unsigned params
        builder.Services.Configure<RouteOptions>(options =>
        {
            options.ConstraintMap.Add("ulong", typeof(ULongRouteConstraint));
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Orange API v1");
            });
        }

        app.UseHttpsRedirection();
        app.UseCors("allowedOrigins");

        app.UseAuthorization();

        app.MapDefaultEndpoints();
        app.MapControllers();

        app.Run();
    }
}