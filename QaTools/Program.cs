
using Microsoft.EntityFrameworkCore;
using QaTools;
using QaTools.Controllers;
using System.Web.Services.Description;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//builder.Services.AddDbContext<Context>(opt =>
//    opt.usesql("name=DefaultConnection"));


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ServicioComandos_Dev.IServicioComandos, ServicioComandos_Dev.ServicioComandosClient>();
builder.Services.AddScoped<ServicioComandos.IServicioComandos, ServicioComandos.ServicioComandosClient>();


var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
