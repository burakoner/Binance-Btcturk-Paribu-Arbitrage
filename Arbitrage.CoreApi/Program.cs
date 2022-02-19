using Arbitrage.CoreApi;
using Arbitrage.CoreApi.Extensions;
using Arbitrage.CoreApi.Services;
using Arbitrage.CoreApi.StreamApi.Exchange;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

#region Configure Services
// Add services to the container.
builder.Services.AddControllers().AddNewtonsoftJson(/*options => { options.SerializerSettings.Converters.Add(new DecimalConverter()); }*/);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

/* Memory Cache */
builder.Services.AddMemoryCache();
// services.AddDistributedMemoryCache();

/* Singleton Objects */
builder.Services.AddSingleton<AppConnections>();
builder.Services.AddSingleton<AppErrors>();
builder.Services.AddSingleton<AppSettings>();
builder.Services.AddSingleton<SocketHandler>();
builder.Services.AddSingleton<AppCache>();

/* Register Hosted Services */
builder.Services.AddSingleton<IHostedService, BinanceTickerService>();
builder.Services.AddSingleton<IHostedService, BtcTurkTickerService>();
builder.Services.AddSingleton<IHostedService, ParibuTickerService>();
builder.Services.AddSingleton<IHostedService, TelegramService>();

/* Lowercase Urls */
builder.Services.AddRouting(options => options.LowercaseUrls = true);

/* Configure CORS */
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
    builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});
#endregion

#region Configure Application
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

/* Https Redirection */
// app.UseHttpsRedirection();

// Use CORS
app.UseCors("CorsPolicy");

/* Routing */
app.UseRouting();

// Gereksiz
// app.UseAuthorization();

// Exception Handler
app.ConfigureExceptionHandler();

/* Endpoints */
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

/* Socket Handler */
var sh = app.Services.GetService<SocketHandler>();
sh.RegisterSocketHandler(app);

app.Run();
#endregion
