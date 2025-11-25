using Microsoft.EntityFrameworkCore;
using software_engineering.Data;
using software_engineering.Lib;
using System.Diagnostics;

//var metrics = new Metrics(@"C:\Users\wc296\AppData\Local\Development\SampleData\GTLB-Data\1c0fd777_20251011.csv");

//Debug.WriteLine(metrics.matrices.Count);
//Debug.WriteLine(metrics.matrices.ElementAt(0));
//Debug.WriteLine(metrics.matrices.ElementAt(0).GetHighPressureRegions(30, 20).Count);

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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// Add the DbContext and specify the database connection
builder.Services.AddDbContext<AppDBContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DBStringConnection")
));

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();