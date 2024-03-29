using Microsoft.IdentityModel.Tokens;
using System.Text;
using ebikeshopserver.Services.Data;
using ebikeshopserver.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ebikeshopserver.Authentication;
using ebikeshopserver.Utils;

namespace ebikeshopserver;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
            options.JsonSerializerOptions.Converters.Add(new ObjectIdJsonConverter()); //להכניס פה
        });

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
                policy.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
            });
        });

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "EbikeShopServer",
                ValidAudience = "EbikeShopFrontEnd",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretSettingsProvider.GetTokenHasher()))
            };
        });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("MustBeAdmin", policy => policy.RequireClaim(Constants.RoleClaimType, "Admin"));
            options.AddPolicy("MustBeSellerOrAdmin", policy => policy.RequireClaim(Constants.RoleClaimType, "Seller", "Admin"));
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

        app.UseAuthentication();

        app.UseMiddleware<RequestsResponsesLogger>();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
