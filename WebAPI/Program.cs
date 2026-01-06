using Application.Services.Implement;
using FluentAssertions.Common;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text.Json.Serialization;
using WebAPI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient<BlynkService>();
builder.Services.AddWebAPIService(builder);
builder.Services.AddInfractstructure(builder.Configuration);
builder.Services.AddSignalR();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

}
app.UseHttpsRedirection();
app.UseCors("_myAllowSpecificOrigins");
app.UseSwagger();
app.UseSwaggerUI();
#region Add sau

app.UseSession();

app.MapHealthChecks("/healthz");



#endregion


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
// Map Hub
app.MapHub<OrderNotificationHub>("/hubs/order-notification");
app.MapHub<NotificationHub.ManagerNotificationHub>("/hubs/manager-notification");
app.Run();
