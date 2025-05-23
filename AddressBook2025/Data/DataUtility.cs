using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AddressBook2025.Data
{
    public class DataUtility
    {
        public static async Task ManageDataAsync(IServiceProvider serviceProvider) 
        {   
            // get db connection
            var dbContextSrc= serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManagerSvc = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var config = serviceProvider.GetRequiredService<IConfiguration>();
            //will sync up db with the model
            await dbContextSrc.Database.MigrateAsync();
        }
        public static async Task SeedDataAsync(IServiceProvider serviceProvider) 
        {   
            // get db connection
            var dbContextSrc= serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManagerSvc = serviceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<ApplicationUser>>();
            var config = serviceProvider.GetRequiredService<IConfiguration>();
            //will sync up db with the model
            await dbContextSrc.Database.MigrateAsync();
            await SeedDemoUserAsync(userManagerSvc, config);
        }
        public static async Task SeedDemoUserAsync(UserManager<ApplicationUser> userManager, IConfiguration config)
        {
            try
            {
                string? demoUserEmail = config["DemoUserLogin"];
                string? demoUserPassword = config["DemoUserPassword"];
                if (string.IsNullOrEmpty(demoUserEmail) || string.IsNullOrEmpty(demoUserPassword)) 
                    throw new ArgumentException("Demo user credentials are not provided in the configuration. Skipping User seeding.");
                ApplicationUser demoUser = new()
                {
                    UserName = demoUserEmail,
                    Email = demoUserEmail,
                    FirstName = "Demo",
                    LastName = "Login",
                    EmailConfirmed = true,
                };
                // Check if the user already exists
                ApplicationUser? user = await userManager.FindByEmailAsync(demoUser.Email);
                if(user is null) await userManager.CreateAsync(demoUser, demoUserPassword);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("**** ERROR ****");
                Console.WriteLine("Error seeding demo user. Please check the configuration for DemoUserLogin and DemoUserPassword.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("**** ERROR ****");
                throw;
            }
        }
    }
}
