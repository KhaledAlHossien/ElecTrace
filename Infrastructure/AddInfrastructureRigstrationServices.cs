using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore; 
using Persistence.Data;

namespace Infrastructure
{
    public static class AddInfrastructureRigstrationServices
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddDbContext<DataContext>(opt =>
                         opt.UseSqlServer(configuration.GetConnectionString("Default")));

            //services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //})
            //      .AddJwtBearer(options =>
            //      {
            //          options.TokenValidationParameters = new TokenValidationParameters
            //          {
            //              ValidateIssuer = true,
            //              ValidateAudience = true,
            //              ValidateLifetime = false,
            //              ValidateIssuerSigningKey = true,

            //              ValidIssuer = configuration["JwtSettings:Issuer"],
            //              ValidAudience = configuration["JwtSettings:Audience"],
            //              IssuerSigningKey = new SymmetricSecurityKey(
            //      Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"])
            //  )
            //          };

            //          options.Events = new JwtBearerEvents
            //          {
            //              OnTokenValidated = async context =>
            //              {
            //                  var db = context.HttpContext.RequestServices
            //                .GetRequiredService<DataContext>();

            //                  var tokenString = context.HttpContext.Request.Headers["Authorization"]
            //                .ToString()
            //                .Replace("Bearer ", "");

            //                  if (string.IsNullOrEmpty(tokenString))
            //                  {
            //                      context.Fail("Missing token");
            //                      return;
            //                  }

            //                  var exists = await db.UserTokens
            //                .AnyAsync(t => t.Token == tokenString);

            //                  if (!exists)
            //                  {
            //                      context.Fail("Token revoked");
            //                  }
            //              },

            //              // 🔥 401: Authentication failed (invalid token / missing token)
            //              OnAuthenticationFailed = context =>
            //              {
            //                  context.NoResult();
            //                  context.Response.StatusCode = 401;
            //                  context.Response.ContentType = "application/json";

            //                  return context.Response.WriteAsync(
            //              System.Text.Json.JsonSerializer.Serialize(new
            //              {
            //                  message = "Authentication failed - invalid token"
            //              })
            //            );
            //              },

            //              // 🔥 401: No token or unauthorized request
            //              OnChallenge = context =>
            //              {
            //                  context.HandleResponse();
            //                  context.Response.StatusCode = 401;
            //                  context.Response.ContentType = "application/json";

            //                  return context.Response.WriteAsync(
            //              System.Text.Json.JsonSerializer.Serialize(new
            //              {
            //                  message = "Authorization required - please login"
            //              })
            //            );
            //              },

            //              // 🔥 403: valid token but no permission
            //              OnForbidden = context =>
            //              {
            //                  context.Response.StatusCode = 403;
            //                  context.Response.ContentType = "application/json";

            //                  return context.Response.WriteAsync(
            //              System.Text.Json.JsonSerializer.Serialize(new
            //              {
            //                  message = "You don't have permission to access this resource"
            //              })
            //            );
            //              }


            //          };
            //      });

            return services;
        }
    }
}
