using JobsBoard.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using JobsBoard.Repostry.RepostryPattern;
using JobsBoard.Repostry;
using JobsBoard.Areas.Identity.Data;
using JobsBoard.DataBaseSeeder;
using JobsBoard.Service;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
        options.JsonSerializerOptions.MaxDepth = 64; // Increase depth if necessary
    });


builder.Services.AddDbContext<JobsBoardContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLCON")));

builder.Services.AddScoped<UserService>();

builder.Services.AddScoped<RepostryPattern<IdentityRole>, Roles>();
builder.Services.AddScoped<DataSeeder>();
builder.Services.AddScoped<RoleManager<IdentityRole>>();



builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//builder.Services.AddDefaultIdentity<JobsBoard.Areas.Identity.Data.JobsBoardUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<JobsBoardContext>();
//builder.Services.AddDefaultIdentity<JobsBoardUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddRoles<IdentityRole>().AddEntityFrameworkStores<JobsBoardContext>();
//builder.Services.AddDefaultIdentity<JobsBoardUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddRoles<IdentityRole>() // This will allow you to use roles
//    .AddEntityFrameworkStores<JobsBoardContext>();
builder.Services.AddDefaultIdentity<JobsBoardUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<JobsBoardContext>();

//builder.Services.AddDefaultIdentity<JobsBoardUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddRoles<IdentityRole>()
//    .AddEntityFrameworkStores<JobsBoardContext>();
builder.Services.Configure<IdentityOptions>(
    options =>
    {
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.Password.RequiredUniqueChars = 0;
        options.Password.RequireDigit = false;
        options.Password.RequireNonAlphanumeric = false;





    });

builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    //options.IdleTimeout = TimeSpan.FromSeconds(10);
  
    options.Cookie.IsEssential = true;
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
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
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.UseSession();


using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;

    // Resolve and execute the DataSeeder
    var dataSeeder = serviceProvider.GetRequiredService<DataSeeder>();
    await dataSeeder.SeedDataAsync();
}

//using (var scope = app.Services.CreateScope())
//{
//    var serviceProvider = scope.ServiceProvider;
//    await SeedRolesAsync(serviceProvider);
//}
app.Run();


//async Task SeedRolesAsync(IServiceProvider serviceProvider)
//{
//    // Resolve RoleManager from the service provider
//    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

//    // Define your default roles
//    var roles = new List<string> { "مدير", "باحث عن عمل", "صاحب عمل" };

//    foreach (var role in roles)
//    {
//        // Check if the role exists; if not, create it
//        if (!await roleManager.RoleExistsAsync(role))
//        {
//            await roleManager.CreateAsync(new IdentityRole(role));
//        }
//    }
//}