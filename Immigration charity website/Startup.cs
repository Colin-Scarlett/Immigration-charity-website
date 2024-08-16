using Immigration_charity_website.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.Google;

[assembly: OwinStartup(typeof(Immigration_charity_website.Startup))]

namespace Immigration_charity_website
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateRolesAndUsers().Wait();
        }

        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Configure the application to use a cookie to store information for the signed-in user
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login")
            });

            // Enable the application to use a cookie to store information about the current user
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = "317519267832-9gojkjnt6rmbr6323d6urpkakcjmrgl0.apps.googleusercontent.com",
                ClientSecret = "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",
                Provider = new GoogleOAuth2AuthenticationProvider()
                {
                    OnAuthenticated = async context =>
                    {
                        // Here you can add custom claims if needed
                        // Add a dummy await to remove warning
                        await Task.FromResult(0);
                    }
                }
            });
        }

        private async Task CreateRolesAndUsers()
        {
            // Create RoleManager and UserManager
            using (var dbContext = ApplicationDbContext.Create())
            {
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(dbContext));
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(dbContext));

                // Create Admin role
                if (!await roleManager.RoleExistsAsync("Admin"))
                {
                    var role = new IdentityRole("Admin");
                    await roleManager.CreateAsync(role);

                    // Create default admin user
                    var user = new ApplicationUser
                    {
                        UserName = "admin@myapp.com",
                        Email = "admin@myapp.com"
                    };
                    string userPWD = "Admin@123456";

                    var createAdminUser = await userManager.CreateAsync(user, userPWD);

                    // Add default Admin user to Role Admin
                    if (createAdminUser.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user.Id, "Admin");
                    }
                }

                // Create Customer role
                if (!await roleManager.RoleExistsAsync("Customer"))
                {
                    var role = new IdentityRole("Customer");
                    await roleManager.CreateAsync(role);
                }
            }
        }
    }
}
