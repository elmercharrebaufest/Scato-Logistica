
using QaTools.Dao;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddScoped<ServicioComandos_Dev.IServicioComandos, ServicioComandos_Dev.ServicioComandosClient>();
builder.Services.AddScoped<ServicioComandos.IServicioComandos, ServicioComandos.ServicioComandosClient>();

builder.Services.AddScoped<IAdministracionDao, AdministracionDao>();


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
