using Amazon;
using Amazon.SQS;
using CloudSupportSystem.Data;
using CloudSupportSystem.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Enable MVC controllers and views
builder.Services.AddControllersWithViews();

// Configure Amazon SQS
builder.Services.AddSingleton<IAmazonSQS>(_ =>
    new AmazonSQSClient(
        RegionEndpoint.GetBySystemName(
            builder.Configuration["AWS:Region"])));

builder.Services.AddScoped<SqsService>();

// Configure database
var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

// Configure HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=SupportTickets}/{action=Index}/{id?}");

app.Run();
