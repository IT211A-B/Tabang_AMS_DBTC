var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".AMS.Session";
    options.Cookie.SameSite = SameSiteMode.Lax;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

// ── Routes ────────────────────────────────────────────────────
// Route 1: handles /AMS/Auth/Login, /AMS/DashBoard/Index, etc.
app.MapControllerRoute(
    name: "ams",
    pattern: "AMS/{controller}/{action}/{id?}");

// Route 2: root / → goes to Auth/Login by default
// DO NOT use app.MapGet("/") — it conflicts with MapControllerRoute
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();
