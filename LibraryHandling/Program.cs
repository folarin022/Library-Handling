using LibraryHandling.Context;
using LibraryHandling.Data;
using LibraryHandling.Repository;
using LibraryHandling.Repository.Interface;
using LibraryHandling.Service;
using LibraryHandling.Service.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --------------------
// Add services
// --------------------

// Configure your DbContext
builder.Services.AddDbContext<LibraryManagementDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity with Roles
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 4;
})
.AddRoles<IdentityRole>()  // ← ADD THIS LINE
.AddEntityFrameworkStores<LibraryManagementDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Add your services here
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IBorrowingRepository, BorrowingRepository>();

var app = builder.Build();

// --------------------
// Seed Admin role & user
// --------------------
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        // Create Admin role if it doesn't exist
        if (!await roleManager.RoleExistsAsync("Admin"))
            await roleManager.CreateAsync(new IdentityRole("Admin"));

        // Create Member role if it doesn't exist
        if (!await roleManager.RoleExistsAsync("Member"))
            await roleManager.CreateAsync(new IdentityRole("Member"));

        // Create default admin user
        string adminEmail = "admin@example.com";
        string adminPassword = "Admin123!";

        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,  // ← ADD THIS
                FirstName = "System",
                LastName = "Admin"
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// --------------------
// Configure middleware
// --------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Identity middleware
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

// Default MVC route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();