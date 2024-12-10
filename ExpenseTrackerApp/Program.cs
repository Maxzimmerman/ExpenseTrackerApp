using ExpenseTrackerApp.Data;
using ExpenseTrackerApp.Data.Repositories;
using ExpenseTrackerApp.Data.Repositories.IRepsitories;
using ExpenseTrackerApp.Services;
using ExpenseTrackerApp.Services.IServices;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

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
        httpsOptions.ServerCertificate = new X509Certificate2("/app/ExpenseTrackerApp/https_certificate/aspnetapp.pfx", "YourCertificatePassword");
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

// Lazy Repositories
builder.Services.AddScoped(typeof(Lazy<ITransactionRepository>), serviceProvider =>
    new Lazy<ITransactionRepository>(() => serviceProvider.GetRequiredService<ITransactionRepository>()));

// Services
builder.Services.AddScoped<IUserManageService, UserManageService>();

// Mail Settings and Mail Serivce
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddScoped<IEmailSender, EmailSender>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.ApplyMigrations();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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