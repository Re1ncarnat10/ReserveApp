using System.Text;
using ReserveApp.Data;
using ReserveApp.Interfaces;
using ReserveApp.Services;
using ReserveApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen(c =>
{
  c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

  c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
  {
          Name = "Authorization",
          Type = SecuritySchemeType.ApiKey,
          Scheme = "Bearer",
          BearerFormat = "JWT",
          In = ParameterLocation.Header,
          Description = "JWT Authorization header using the Bearer scheme."
  });
  c.AddSecurityRequirement(new OpenApiSecurityRequirement
  {
          {
                  new OpenApiSecurityScheme
                  {
                          Reference = new OpenApiReference
                          {
                                  Type = ReferenceType.SecurityScheme,
                                  Id = "Bearer"
                          }
                  },
                  Array.Empty<string>()
          }
  });
});

builder.Services.AddCors(options =>
{
        options.AddPolicy("AllowAllOrigins", builder =>
        { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
});
});

builder.Services.AddScoped<ILoginAndRegisterService, LoginAndRegisterService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IResourceService, ResourceService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserResourceService, UserResourceService>();
builder.Services.AddHostedService<ResourceAvailabilityService>();

builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<User, IdentityRole>(options =>
        {
          options.Password.RequireDigit = true;
          options.Password.RequireLowercase = true;
          options.Password.RequireUppercase = true;
          options.Password.RequiredLength = 8;
          options.Password.RequireNonAlphanumeric = false;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

var jwtSettings = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSettings["Key"] ?? "DefaultKey";
var key = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
        {
          options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
          options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
          options.TokenValidationParameters = new TokenValidationParameters
          {
                  ValidateIssuer = true,
                  ValidateAudience = true,
                  ValidateLifetime = true,
                  ValidateIssuerSigningKey = true,
                  ValidIssuer = jwtSettings["Issuer"],
                  ValidAudience = jwtSettings["Audience"],
                  IssuerSigningKey = new SymmetricSecurityKey(key)
          };
          options.Events = new JwtBearerEvents
          {
                  OnAuthenticationFailed = context =>
                  {
                    var logger = context.HttpContext.RequestServices
                            .GetRequiredService<ILogger<Program>>();
                    logger.LogError(
                            $"Authentication failed. Exception: {context.Exception.Message}");
                    var sanitizedMessage = Regex.Replace(context.Exception.Message,
                            @"[^\u0020-\u007E]", string.Empty);
                    context.Response.Headers.Append("Authentication-Failed", sanitizedMessage);
                    return Task.CompletedTask;
                  }
          };
        });
builder.Services.AddControllers();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
  var services = scope.ServiceProvider;
  var dbContext = services.GetRequiredService<AppDbContext>();
  dbContext.Database.Migrate();
  var loginAndRegisterService = services.GetRequiredService<ILoginAndRegisterService>();
  await loginAndRegisterService.CreateRoles();
  var userService = scope.ServiceProvider.GetRequiredService<IAdminService>();
  await userService.InitializeAdminAsync();
}

if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Error");
  app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
  c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
  c.RoutePrefix = string.Empty;
});

app.UseCors("AllowAllOrigins");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseHttpsRedirection();
app.Run();