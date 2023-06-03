using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using ShopM4_DataMigrations.Repository;
using ShopM4_DataMigrations.Data;
using ShopM4_DataMigrations.Repository.IReporitory;
using ShopM4_Utility;
using ShopM4_Models;
using ShopM4_Utility.BrainTree;

var builder = WebApplication.CreateBuilder(args);
//builder.Services.AddDbContext<ApplicationDbContext>(
//    options => options.(
//        builder.Configuration.GetConnectionString("DefaultConnection"))
//    );

builder.Services
        .AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
        });

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationDbContext>();

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationDbContext>();
//builder.Services.AddDefaultIdentity<IdentityUser>()
//    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddDefaultUI().AddDefaultTokenProviders()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddAuthentication().AddGoogle(googleOptions =>
{
    googleOptions.ClientId = "240179620513-2d4nr5gvn1pva7nkmbler8g9mphifobn.apps.googleusercontent.com";
    googleOptions.ClientSecret = "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";
});

builder.Services.AddAuthentication().AddFacebook(options =>
{
    options.AppId = "242921524810422";
    options.AppSecret = "add412954f87224d68b97782327b3eed";
});


// Add services to the container.
builder.Services.AddHttpContextAccessor();//����� ��� ������ � �������� �� ���

builder.Services.AddScoped<IRepositoryCategory, RepositoryCategory>();
builder.Services.AddScoped<IRepositoryMyModel, RepositoryMyModel>();
builder.Services.AddScoped<IRepositoryProduct, RepositoryProduct>();
builder.Services.AddScoped<IRepositoryQueryDetail, RepositoryQueryDeatail>();
builder.Services.AddScoped<IRepositoryQueryHeader, RepositoryQueryHeader>();
builder.Services.AddScoped<IRepositoryOrderHeader, RepositoryOrderHeader>();
builder.Services.AddScoped<IRepositoryOrderDetail, RepositoryOrderDetail>();
builder.Services.AddScoped<IRepositoryApplicationUser, RepositoryApplicationUser>();

builder.Services.AddSession(options =>
{
    options.Cookie.Name = "Winter2022";
    //options.IdleTimeout = TimeSpan.FromSeconds(10);
}); // ��������� ������ ������
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddControllersWithViews();
builder.Services.Configure<SettingsBrainTree>(
    builder.Configuration.GetSection("BrainTree"));
builder.Services.AddTransient<IBrainTreeBridge, BrainTreeBridge>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
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
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.MapRazorPages();
//app.Use((context, next) =>
//{
//    context.Items["name"] = "Danya";
//    return next.Invoke();
//});

app.UseSession(); // ��������� ������ ������

//app.Run(x =>
//{
//    // return x.Response.WriteAsync("Hello "+ x.Items["name"]);
//    if (x.Session.Keys.Contains("name"))
//    {
//        return x.Response.WriteAsync(x.Session.GetString("name"));
//    }
//    else
//    {
//        x.Session.SetString("name", "Danya");
//        return x.Response.WriteAsync("No");
//    }
//});
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<ApplicationDbContext>();
    if (context.Database.GetPendingMigrations().Any())
    {
        context.Database.Migrate();
    }
}
app.Run();
