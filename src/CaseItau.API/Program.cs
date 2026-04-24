using CaseItau.API.Extensions;
using CaseItau.API.Middlewares;
using CaseItau.Application;
using CaseItau.Infra;
using CaseItau.Infra.Data.Seed;
using Serilog;
using System.Text.Json.Serialization;

public class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseSerilog((hostingContext, configuration) =>
            configuration.ReadFrom.Configuration(hostingContext.Configuration));

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerDocumentation();

        builder.Services.AddControllers()
            .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        builder.Services.AddApplication();

        builder.Services.AddInfra(builder.Configuration);

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.ApplyMigrations();
        }
        await DatabaseSeeder.SeedData(app.Services);

        app.UseExceptionHandler();

        app.UseHttpsRedirection();

        app.UseMiddleware<RequestContextLoggingMiddleware>();
        app.UseSerilogRequestLogging();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}