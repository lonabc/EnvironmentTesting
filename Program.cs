using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebSockets;
using TempModbusProject.Configure;
using TempModbusProject.Configure.FilterConfigure;
using TempModbusProject.Model;
using TempModbusProject.Service;
using TempModbusProject.Service.Context;
using TempModbusProject.Service.IService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//设置配置文件
ConfigurationBuilder configBuilder = new ConfigurationBuilder();
configBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
IConfigurationRoot config = configBuilder.Build();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JWT"));//读取配置文件
builder.Services.Configure<SqlSettings>(builder.Configuration.GetSection("ConnectionStrings"));//读取配置文件
builder.Services.Configure<MvcOptions>(options =>
{
    options.Filters.Add<AllExceptionFilter>(); //注册全局异常过滤器
    options.Filters.Add<TransactionFilter>(); //注册全局事务过滤器
    options.Filters.Add<currentLimit>(); //注册全局限流过滤器
});
//注册服务
builder.Services.AddScoped<PortLineVm>();
builder.Services.AddScoped<readConfig>();
builder.Services.AddScoped<SqlToolsServices>();
builder.Services.AddScoped<IConsoleChangeColors, ConsoleChangeColor>();
builder.Services.AddSingleton<IModbusFactory,ModbusFactory>();
builder.Services.AddSingleton<Esp8266Connect>();
builder.Services.AddScoped<CommunicationImp>();
builder.Services.AddSingleton<ModbusPublicBasic>();
// 注册定时任务服务
builder.Services.AddHostedService<TimeTaskImp>();

builder.Services.AddMemoryCache();
builder.Services.AddScoped<ICommunicationFactory, CommunicationFactory>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
