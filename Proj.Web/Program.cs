using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Proj.DataAccess.Data;
using Proj.DataAccess.Repository;
using Proj.DataAccess.Repository.IRepository;
using Proj.Utility;
using Proj.Web.Services;

#region ------------------- Builder -------------------
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


//--------- 1. Register DbContext --------------
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


//--------- 2. Testing Services LifeTime --------------
builder.Services.AddSingleton<ISingleTonGuidService, SingleTonGuidService>();
builder.Services.AddScoped<IScopedGuidService, ScopedGuidService>();
builder.Services.AddTransient<ITransientGuidService, TransientGuidService>();
builder.Services.AddTransient<IEmailSender, EmailSender>();


//--------- 3. Registrering Repositoriers -------------- so many Repository  complex
//builder.Services.AddScoped<ICategoryRepository,CategoryRepository>();
//builder.Services.AddScoped<IProductRepository,ProductRepository>();
//builder.Services.AddScoped<IProductRepository,ProductRepository>();
//builder.Services.AddScoped<IProductRepository,ProductRepository>();
//builder.Services.AddScoped<IProductRepository,ProductRepository>();

//--------- 4. Registrering IunitOfWork --------------
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();

//--------- 5. Adding Identity --------------
//____ only Identity ____
/*builder.Services.AddDefaultIdentity<IdentityUser>()//options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();*/

//__ only Identity __
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

//__ Identity Path __
builder.Services.ConfigureApplicationCookie(option =>
{
    option.LoginPath = $"/Identity/Account/Login";
    option.LogoutPath = $"/Identity/Account/Logout";
    option.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});


builder.Services.AddRazorPages();
#endregion


#region ------------------- App -------------------

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

//_____ For Identity Razar Pages _______
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    //_____ For simpel Route ______
    //pattern: "{controller=Home}/{action=Index}/{id?}");

    //_____ For AreaBase Route ______
    //pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

    //_____ For Fixedd AreaBase Route ______
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();

#endregion