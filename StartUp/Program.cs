using Application;
using Auth0;
using EwwDatabase;
using SignalR;
using StartUp.Configurations;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Services.BuildCustomConfiguration();

builder.Services.AddEwwDatabaseServices(configuration);
builder.Services.AddAuth0Services(configuration);

builder.Services.AddApplicationServices();

builder.Services.AddSignalRServices();

var app = builder.Build();

app.UseSignalRConfigurations();

app.Run();