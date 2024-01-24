using System.Text;
using API.Data;
using API.Entities;
using API.Extensions;
using API.Intefaces;
using API.MiddleWares;
using API.services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddIdentityCore<AppUser>(opt=>
    {
     opt.Password.RequireNonAlphanumeric=false;

    }).AddRoles<AppRoles>().AddRoleManager<RoleManager<AppRoles>>().AddEntityFrameworkStores<DataContext>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
    Options=>{
        Options.TokenValidationParameters= new TokenValidationParameters{
            ValidateIssuerSigningKey=true,
            IssuerSigningKey= new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["TokenKey"])),
            ValidateIssuer=false,
            ValidateAudience=false
        };
    }
);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleWare>();
app.UseCors(builder=>builder.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200"));

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
 using var scope = app.Services.CreateScope();
 var service = scope.ServiceProvider;
 try{
    var context = service.GetRequiredService<DataContext>();
    await context.Database.MigrateAsync();
    // await Seed.SeedUsers(context);
 }catch(Exception ex){
    var logger = service.GetService<ILogger<Program>>();
    logger.LogError(ex,"an error occured during migration");
 }

app.Run();
