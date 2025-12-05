using Microsoft.EntityFrameworkCore;
using software_engineering.Data;
using software_engineering.Lib;
using System.Diagnostics;

//var metrics = new Metrics(@"C:\Users\wc296\AppData\Local\Development\SampleData\GTLB-Data\1c0fd777_20251011.csv");

//Debug.WriteLine("first matrix:");
//metrics.matrices.ElementAt(0).Print();

//foreach (List<PressurePoint> area in metrics.matrices.ElementAt(0).GetHighPressureRegions(30, 20))
//{
//    string areaString = "";

//    foreach (PressurePoint point in area)
//    {
//        areaString += "(" + point.x.ToString() + ", " + point.y.ToString() + "), ";
//    }

//    Debug.WriteLine(areaString);
//}

//Debug.WriteLine("Peak pressure index:");
//Debug.WriteLine(metrics.matrices.ElementAt(0).GetPeakPressureIndex());
//Debug.WriteLine("Contact Area Percentage:");
//Debug.WriteLine(metrics.matrices.ElementAt(0).GetContactAreaPercentage().ToString() + "%");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// Add the DbContext and specify the database connection
builder.Services.AddDbContext<AppDBContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DBStringConnection")
));
builder.Services.AddDistributedMemoryCache();
//allows sesons support for storing login in id and roles
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
var app = builder.Build();

app.UseSession();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Users}/{action=Login}/{id?}"
);

app.Run();