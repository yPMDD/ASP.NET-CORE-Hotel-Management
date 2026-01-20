using HotelAurelia.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using HotelAurelia.Services;


var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("HotelAureliaContextConnection")
    ?? throw new InvalidOperationException("Connection string not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDbContext<HotelAureliaDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddTransient<IEmailSender, SmtpEmailSender>();

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders(); // IMPORTANT

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// In Program.cs, add this service
//builder.Services.AddSingleton<IEmailSender, ConsoleEmailSender>();

var app = builder.Build();

// 🔹 Seed Identity data correctly
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await RoleSeeder.SeedAsync(services);
    await IdentitySeeder.SeedAdminAsync(services);
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//  REQUIRED for Identity
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
