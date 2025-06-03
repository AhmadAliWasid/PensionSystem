using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PensionSystem.Data;
using PensionSystem.Helpers;
using PensionSystem.Interfaces;
using PensionSystem.Services;
using Serilog;
using WebAPI.Interfaces;
using WebAPI.Mappings;
using WebAPI.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("Server");
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
});

builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ContractResolver = new DefaultContractResolver();
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    });

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
builder.Services.AddScoped<IWWFSanction, WWFSanctionService>();
builder.Services.AddScoped<IWWFReimbursment, WWFReimbursmentService>();
builder.Services.AddScoped<SessionHelper>();
builder.Services.AddHttpClient();
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8); // Use a single session configuration
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();
builder.Services.AddRequestTimeouts();

// logging using serilog
var _logger = new LoggerConfiguration()
    .MinimumLevel.Error()
    .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
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
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PESCO Pension System V2"));
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSerilogRequestLogging(); // Move this up for better logging

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
app.MapRazorPages();

app.Run();