using Microsoft.IdentityModel.Tokens;
using System.Text;
using ebikeshopserver.Services.Data;

namespace ebikeshopserver;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddSingleton(serviceProvider =>
        {
            var config = serviceProvider.GetService<IConfiguration>();
            return MongoDbService.CreateMongoClient(config);
        });

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("LocalCorsPolicy", policy =>
            {
                policy.WithOrigins("http://localhost:3000")
                .AllowAnyMethod()
                .AllowAnyHeader();
            });
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors("LocalCorsPolicy");

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
