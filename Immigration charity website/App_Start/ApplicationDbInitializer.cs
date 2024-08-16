using Immigration_charity_website.Models;
using Immigration_charity_website;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.Data.Entity;

public class ApplicationDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
{
    protected override void Seed(ApplicationDbContext context)
    {
        InitializeIdentityForEF(context);
        base.Seed(context);
    }

    public static void InitializeIdentityForEF(ApplicationDbContext context)
    {
        var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
        var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

        string[] roleNames = { "Admin", "User" };
        IdentityResult roleResult;

        foreach (var roleName in roleNames)
        {
            if (!roleManager.RoleExists(roleName))
            {
                roleResult = roleManager.Create(new IdentityRole(roleName));
            }
        }

        const string adminEmail = "admin@example.com";
        const string adminPassword = "Admin@123456";

        var adminUser = userManager.FindByName(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser { UserName = adminEmail, Email = adminEmail, Role = "Admin" };
            var result = userManager.Create(adminUser, adminPassword);
            result = userManager.SetLockoutEnabled(adminUser.Id, false);
        }

        var rolesForAdmin = userManager.GetRoles(adminUser.Id);
        if (!rolesForAdmin.Contains("Admin"))
        {
            var result = userManager.AddToRole(adminUser.Id, "Admin");
        }
    }
}
