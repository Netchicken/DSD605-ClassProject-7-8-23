using DSD605ClassProject.Data;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
     .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages(

    options =>
    {
        //lock out the entire roles folder to someone who doesn't meet the AdminViewPolicy policy.
        options.Conventions.AuthorizeFolder("/RolesManager", "AdminViewPolicy");
        options.Conventions.AuthorizeFolder("/ClaimsManager", "AdminViewPolicy");
        //  options.Conventions.AuthorizePage("/ClaimsManager/Assign", "SuperAdminRolePolicy");
    }


    ); //add this in to make razorpages work

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SuperAdminRolePolicy", policyBuilder => policyBuilder.RequireRole("Admin"));
    options.AddPolicy("AdminRolePolicy", policyBuilder => policyBuilder.RequireRole("Admin"));
    options.AddPolicy("AdminClaimPolicy", policyBuilder => policyBuilder.RequireClaim("SuperAdmin"));

    options.AddPolicy("AdminViewPolicy", policyBuilder => policyBuilder.RequireAssertion(context =>
    {
        var joiningDateClaim = context.User.FindFirst(c => c.Type == "Joining Date")?.Value; //comes back as string
        var joiningDate = Convert.ToDateTime(joiningDateClaim); //now we have the joining date as a datetime object

        return context.User.HasClaim(c => c.Type == "SuperAdmin") && context.User.HasClaim("Permission", "View Roles") && joiningDate < DateTime.Now.AddMonths(-6);

    }));


    //context.User.HasClaim("Permission", "View Roles") &&
    //joiningDate < DateTime.Now.AddMonths(-6) &&

});

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 4;

    options.Password.RequiredUniqueChars = 1;
    options.SignIn.RequireConfirmedEmail = false;

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings. 
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;

});

builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseSwagger();
    app.UseSwaggerUI();
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
app.UseAuthorization();//Authorization middleware is enabled by default in the web application template by the inclusion of app.UseAuthorization() in the Program class.

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllers();
app.MapRazorPages();

app.Run();
