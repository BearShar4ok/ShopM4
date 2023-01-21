using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using ShopM4.Data;
using ShopM4.Utility;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"))
    );
//builder.Services.AddDefaultIdentity<IdentityUser>()
//    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddDefaultUI().AddDefaultTokenProviders()
    .AddEntityFrameworkStores<ApplicationDbContext>();
// Add services to the container.
builder.Services.AddHttpContextAccessor();//����� ��� ������ � �������� �� ���

builder.Services.AddSession(options =>
{
    options.Cookie.Name = "Winter2022";
    //options.IdleTimeout = TimeSpan.FromSeconds(10);
}); // ��������� ������ ������
builder.Services.AddTransient<IEmailSender,EmailSender>();
builder.Services.AddControllersWithViews();

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

app.Run();
