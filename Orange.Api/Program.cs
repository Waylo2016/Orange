using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi;
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


        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: "allowedOrigins",
                policy =>
            {
                policy
                    .WithOrigins("http://localhost:8080")
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
                    Email = new string("Waylo@waylo.tech")
                }
            });
            string xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
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