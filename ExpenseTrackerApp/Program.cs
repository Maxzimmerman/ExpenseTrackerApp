using System.Runtime.CompilerServices;
using ExpenseTrackerApp.Data;
using ExpenseTrackerApp.Data.Repositories;
using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using ExpenseTrackerApp.Services;
using ExpenseTrackerApp.Services.IServices;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;
using ExpenseTrackerApp.Models;
using ExpenseTrackerApp.SeedDataBase;
using ExpenseTrackerApp.SeedDataBase.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("Database")));

builder.Services.AddDefaultIdentity<IdentityUser>(options => {
    options.SignIn.RequireConfirmedAccount = true;
    options.User.RequireUniqueEmail = true;
    options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/UserManage/SignIn";
    options.LogoutPath = "/UserManage/SignOut";
    options.AccessDeniedPath = "/UserManage/AccessDenied";
    options.SlidingExpiration = true;
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.ConfigureHttpsDefaults(httpsOptions =>
    {
        httpsOptions.ServerCertificate = new X509Certificate2(
            "/app/ExpenseTrackerApp/https_certificate/aspnetapp.pfx", 
            "YourCertificatePassword");
    });
});



builder.Services.AddControllersWithViews();

// Repositories
builder.Services.AddScoped<IFooterRepository, FooterRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IFooterRepository, FooterRepository>();
builder.Services.AddScoped<IBudgetRepository, BudgetRepository>();
builder.Services.AddScoped<ISocialLinksRepository, SocialLinksRepository>();
builder.Services.AddScoped<ICategoryTypeRepsitory, CategoryTypeRepository>();
builder.Services.AddScoped<ICategoryIconRepository, CategoryIconRepository>();
builder.Services.AddScoped<ICategoryColorRepository, CategoryColorRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IWalletRepository, WalletRepository>();

// Background Services
builder.Services.AddHostedService<SyncTransactionsBackgroundService>();

// Lazy Repositories
builder.Services.AddScoped(typeof(Lazy<ITransactionRepository>), serviceProvider =>
    new Lazy<ITransactionRepository>(() => serviceProvider.GetRequiredService<ITransactionRepository>()));
builder.Services.AddScoped(typeof(Lazy<IBudgetRepository>), serviceProvider =>
    new Lazy<IBudgetRepository>(() => serviceProvider.GetRequiredService<IBudgetRepository>()));

// Services
builder.Services.AddScoped<IUserManageService, UserManageService>();
builder.Services.AddScoped<IPlaidService, PlaidService>();

// Mail Settings and Mail Serivce
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddScoped<IEmailSender, EmailSender>();

var app = builder.Build();

//Configure the HTTP request pipeline.
using (var scope = app.Services.CreateScope())
{
     var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
     var userManagerService = scope.ServiceProvider.GetRequiredService<IUserManageService>();
     context.Database.Migrate();
     
     //var seedCategory = new SeedCategory(context, userManagerService);
     //var seedFooter = new SeedFooter(context);
     //var seedSocialLinks = new SeedSocialLinks(context);
     //    
     //seedFooter.ReadCSV();
     //seedSocialLinks.ReadCSV();
     //seedCategory.Seed();
}

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint(); // Optional for dev
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Home}/{id?}");
app.MapRazorPages();

app.Run();
