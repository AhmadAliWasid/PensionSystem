using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PensionSystem.Data;
using PensionSystem.Helpers;
using PensionSystem.Interfaces;
using PensionSystem.Mappings;
using PensionSystem.Services;
using Serilog;
using WebAPI.Services;


var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("Local"); // Local || 
builder.Services.AddDbContext<ApplicationDbContext>(options =>
  options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services
    .AddDefaultIdentity<IdentityUser>(o =>
    o.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
builder.Services.Configure<IdentityOptions>(o =>
{
    o.Password.RequiredLength = 8;
    o.Password.RequiredUniqueChars = 1;
    o.Password.RequireDigit = false;
    o.Password.RequireLowercase = false;
    o.Password.RequireUppercase = false;
    o.Password.RequireNonAlphanumeric = false;
    o.SignIn.RequireConfirmedEmail = true;
}
);
builder.Services.AddControllersWithViews();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IPensioner, PensionerService>();
builder.Services.AddScoped<IHBLArrears, HBLArrearsService>();
builder.Services.AddScoped<ICompany, CompanyService>();
builder.Services.AddScoped<ICommutation, CommutationService>();
builder.Services.AddScoped<IHBLPayments, HBLPaymentService>();
builder.Services.AddScoped<IArrearsDemand, ArrearsDemandService>();
builder.Services.AddScoped<IArrearsPayment, ArrearsPaymentService>();
builder.Services.AddScoped<ICheque, ChequeService>();
builder.Services.AddScoped<IRelation, RelationService>();
builder.Services.AddScoped<IMonthlyPensionDemand, MonthlyPensionDemandService>();
builder.Services.AddScoped<IRestorationDemand, RestorationDemandService>();
builder.Services.AddScoped<IRestorationPayment, RestorationPaymentService>();
builder.Services.AddScoped<IPensionPayment, PensionPaymentService>();
builder.Services.AddScoped<IChequeCategory, ChequeCategoryService>();
builder.Services.AddScoped<IBank, BankServices>();
builder.Services.AddScoped<IBranch, BranchService>();
builder.Services.AddScoped<IPDU, PDUService>();
builder.Services.AddScoped<IUserPDU, UserPDUService>();
builder.Services.AddScoped<ICashBook, CashBookService>();
builder.Services.AddHttpClient();
// automapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));
// scoped
builder.Services.AddScoped<SessionHelper>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddMvc()
        .AddSessionStateTempDataProvider();
// swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8); // Adjust the timeout as needed
});
builder.Services.AddMvc()
    .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
builder.Services.AddControllers()
    .AddNewtonsoftJson(
    o => o.SerializerSettings.ContractResolver = new DefaultContractResolver() { });
builder.Services.AddControllers()
    .AddNewtonsoftJson(
    o => o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
// session
builder.Services.AddSession(o => { o.IdleTimeout = TimeSpan.FromMinutes(120); });
builder.Services.AddRequestTimeouts();
// logging using serilog
var _logger = new LoggerConfiguration()
    .MinimumLevel
    .Error()
    .WriteTo
    .File("Logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Services.AddSerilog(_logger);
var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();

}
else
{
    app.UseMigrationsEndPoint();
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
// swagger
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PESCO Pension System V2"));
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// session
app.UseRequestTimeouts();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors();
app.MapAreaControllerRoute(
    name: "MyAreaClaimant",
    areaName: "Claimant",
    pattern: "Claimant/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.UseEndpoints(endpoints =>
{
    endpoints.MapBlazorHub();
    endpoints.MapFallbackToPage("/_Host");
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.MapRazorPages();

app.UseSerilogRequestLogging();

app.Run();