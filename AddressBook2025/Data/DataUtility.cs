using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Bogus;
using AddressBook2025.Models;
using AddressBook2025.Client.Models.Enums;
using Npgsql;

namespace AddressBook2025.Data
{
    public class DataUtility
    {
        public static string GetConnectionString(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DbConnection");//Local connection string
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_PRIVATE_URL");//Railway connection string
            return string.IsNullOrEmpty(databaseUrl) ? connectionString! : BuildConnectionString(databaseUrl); 
        }

        private static string BuildConnectionString(string databaseUrl)
        {
            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database= databaseUri.LocalPath.TrimStart('/'),
                SslMode = SslMode.Prefer

            };
                return builder.ToString();
        }
        public static async Task ManageDataAsync(IServiceProvider svc)
        {
            var db = svc.GetRequiredService<ApplicationDbContext>();
            var um = svc.GetRequiredService<UserManager<ApplicationUser>>();

            await db.Database.MigrateAsync();

            /* ---- Demo account (kept exactly as before) ---- */
            var config = svc.GetRequiredService<IConfiguration>();
            var demoEmail = config["DemoUserLogin"]!;
            var demoPassword = config["DemoUserPassword"]!;
            var demoUser = await EnsureUserAsync(um, demoEmail, demoPassword, "Demo", "Login");
            await SeedContactsForUserAsync(demoUser, db);

            /* ---- Wanda ---- */
            var wandaUser = await EnsureUserAsync(
                um,
                email: "wanda@mailinator.com",
                password: "Abc&123!",
                firstName: "Wanda",
                lastName: "Maximoff");
            await SeedContactsForUserAsync(wandaUser, db);
        }

        public static async Task SeedDataAsync(IServiceProvider serviceProvider)
        {
            // get db connection
            var dbContextSrc = serviceProvider.GetRequiredService<ApplicationDbContext>();
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
                    PhoneNumber = "5555551212",
                    EmailConfirmed = true,
                };
                // Check if the user already exists
                ApplicationUser? user = await userManager.FindByEmailAsync(demoUser.Email);
                if (user is null) await userManager.CreateAsync(demoUser, demoUserPassword);

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

        public static async Task SeedDemoContactsAsync(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IConfiguration config)
        {
            string? demoUserEmail = config["DemoUserLogin"];

            // No demo user email provided, skip seeding contacts
            if (string.IsNullOrEmpty(demoUserEmail)) return;

            var user = await userManager.FindByEmailAsync(demoUserEmail);
            // No demo user found, skip seeding contacts
            if (user is null) return;

            var demoContacts = await context.Contacts
            .Where(c => c.AppUserId == user.Id)
            .Include(c => c.Categories)
            .ToListAsync();

            var demoCategories = await context.Categories
                .Where(c => c.AppUserId == user.Id)
                .ToListAsync();

            Random rand = new Random();

            if (demoContacts.Count == 0)
            {
                var newContacts = new Faker<Contact>()
                     .RuleFor(c => c.LastName, f => f.Name.LastName())
                     .RuleFor(c => c.BirthDate,
         (f, c) => DateOnly.FromDateTime(
                       f.Date.Between(
                           DateTime.Today.AddYears(-60), // oldest = 60 y
                           DateTime.Today.AddYears(-18)  // youngest = 18 y
                       )))

                     .RuleFor(c => c.PhoneNumber, f => f.Phone.PhoneNumber())
                     .RuleFor(c => c.Address1, f => f.Address.StreetAddress())
                     .RuleFor(c => c.City, f => f.Address.City())
                     .RuleFor(c => c.State, f => f.PickRandom<State>())
                     .RuleFor(c => c.ZipCode, f => f.Address.ZipCode("#####"))
                     .RuleFor(c => c.AppUserId, user.Id)
                     .Generate(10);

                Faker faker = new();
                var imageDir = Path.Combine(Directory.GetCurrentDirectory(), "Data/DemoImages");
                var mensPics = Directory.GetFiles(Path.Combine(imageDir, "Men/")).ToList();
                var WomensPics = Directory.GetFiles(Path.Combine(imageDir, "Women/")).ToList();

                for (int i = 0; i < newContacts.Count; i++)
                {
                    Contact contact = newContacts[i];
                    if (i % 2 == 0)
                    {
                        contact.FirstName = faker.Name.FirstName(Bogus.DataSets.Name.Gender.Male);

                        if (mensPics.Count > 0)
                        {
                            var pic = mensPics[rand.Next(0, mensPics.Count)];
                            mensPics.Remove(pic);

                            ImageUpload image = new()
                            {
                                Data = await File.ReadAllBytesAsync(pic),
                                Type = $"image/{Path.GetExtension(pic).TrimStart('.')}"
                            };
                            contact.Image = image;
                            context.Images.Add(image);
                        }
                    }
                    else
                    {
                        contact.FirstName = faker.Name.FirstName(Bogus.DataSets.Name.Gender.Female);

                        if (WomensPics.Count > 0)
                        {
                            var pic = WomensPics[rand.Next(0, WomensPics.Count)];
                            WomensPics.Remove(pic);

                            ImageUpload image = new()
                            {
                                Data = await File.ReadAllBytesAsync(pic),
                                Type = $"image/{Path.GetExtension(pic).TrimStart('.')}"
                            };
                            contact.Image = image;
                            context.Images.Add(image);
                        }
                    }
                    contact.Email = faker.Internet.Email(contact.FirstName, contact.LastName, "mailinator.com");

                    if (rand.Next() % 2 == 0) contact.Address2 = faker.Address.SecondaryAddress();
                }


                demoContacts.AddRange(newContacts);

            }

            if (demoCategories.Count == 0)
            {
                demoCategories = [
                    new() { Name = "Family", AppUserId = user.Id },
                    new() { Name = "Friends", AppUserId = user.Id },
                    new() { Name = "Coworkers", AppUserId = user.Id },
                    new() { Name = "Clients", AppUserId = user.Id },
                    new() { Name = "Coders", AppUserId = user.Id },
                    new() { Name = "Other", AppUserId = user.Id },
                    ];
                context.Categories.AddRange(demoCategories);
            }

            foreach (var contact in demoContacts.Where(c => c.Categories.Count == 0))
            {
                int numCategories = rand.Next(1, 5);
                var categories = demoCategories
                    .OrderBy(c => Guid.NewGuid())
                    .Take(numCategories);
                contact.Categories = [.. categories];
                context.Update(contact);
            }

            // Save changes to the database
            await context.SaveChangesAsync();
        }
                private static async Task<ApplicationUser> EnsureUserAsync(
                                     UserManager<ApplicationUser> userManager,
                                        string email, string password,
                                        string firstName, string lastName)
                {
                            var user = await userManager.FindByEmailAsync(email);
                            if (user is null)
                            {
                                user = new ApplicationUser
                                {
                                    UserName = email,
                                    Email = email,
                                    FirstName = firstName,
                                    LastName = lastName,
                                    EmailConfirmed = true
                                };
                                await userManager.CreateAsync(user, password);
                            }
                            return user;
                }

                private static async Task SeedContactsForUserAsync(
                    ApplicationUser user,
                    ApplicationDbContext context)
                {
                    // fetch existing data (if any)
                    var userContacts = await context.Contacts
                        .Where(c => c.AppUserId == user.Id)
                        .Include(c => c.Categories)
                        .ToListAsync();

                    var userCategories = await context.Categories
                        .Where(c => c.AppUserId == user.Id)
                        .ToListAsync();

                    Random rand = new();

                    /* ----------  contacts  ---------- */
                    if (userContacts.Count == 0)
                    {
                        var newContacts = new Faker<Contact>()
                            .RuleFor(c => c.LastName, f => f.Name.LastName())
                            .RuleFor(c => c.BirthDate,
                                (f, c) => DateOnly.FromDateTime(
                                    f.Date.Between(
                                        DateTime.Today.AddYears(-60),
                                        DateTime.Today.AddYears(-18))))
                            .RuleFor(c => c.PhoneNumber, f => f.Phone.PhoneNumber())
                            .RuleFor(c => c.Address1, f => f.Address.StreetAddress())
                            .RuleFor(c => c.City, f => f.Address.City())
                            .RuleFor(c => c.State, f => f.PickRandom<State>())
                            .RuleFor(c => c.ZipCode, f => f.Address.ZipCode("#####"))
                            .RuleFor(c => c.AppUserId, user.Id)
                            .Generate(10);

                        // gender-specific images (same logic you already use)
                        Faker faker = new();
                        var imageDir = Path.Combine(Directory.GetCurrentDirectory(), "Data/DemoImages");
                        var mensPics = Directory.GetFiles(Path.Combine(imageDir, "Men")).ToList();
                        var womensPic = Directory.GetFiles(Path.Combine(imageDir, "Women")).ToList();

                        for (int i = 0; i < newContacts.Count; i++)
                        {
                            var contact = newContacts[i];
                            bool isMale = i % 2 == 0;

                            contact.FirstName = isMale
                                ? faker.Name.FirstName(Bogus.DataSets.Name.Gender.Male)
                                : faker.Name.FirstName(Bogus.DataSets.Name.Gender.Female);

                            var pool = isMale ? mensPics : womensPic;
                            if (pool.Count > 0)
                            {
                                var pic = pool[rand.Next(pool.Count)];
                                pool.Remove(pic);
                                contact.Image = new ImageUpload
                                {
                                    Data = await File.ReadAllBytesAsync(pic),
                                    Type = $"image/{Path.GetExtension(pic).TrimStart('.')}"
                                };
                                context.Images.Add(contact.Image);
                            }

                            contact.Email = faker.Internet.Email(contact.FirstName, contact.LastName, "mailpro.live");
                            if (rand.Next() % 2 == 0) contact.Address2 = faker.Address.SecondaryAddress();
                        }

                        userContacts.AddRange(newContacts);
                    }

                    /* ----------  categories  ---------- */
                    if (userCategories.Count == 0)
                    {
                        userCategories =
                        [
                            new() { Name = "Family",    AppUserId = user.Id },
                    new() { Name = "Friends",   AppUserId = user.Id },
                    new() { Name = "Coworkers", AppUserId = user.Id },
                    new() { Name = "Clients",   AppUserId = user.Id },
                    new() { Name = "Coders",    AppUserId = user.Id },
                    new() { Name = "Other",     AppUserId = user.Id },
                ];
                        context.Categories.AddRange(userCategories);
                    }

                    /* ----------  join contacts to categories  ---------- */
                    foreach (var contact in userContacts.Where(c => c.Categories.Count == 0))
                    {
                        var cats = userCategories
                            .OrderBy(_ => Guid.NewGuid())
                            .Take(rand.Next(1, 5));
                        contact.Categories = [.. cats];
                        context.Update(contact);
                    }

                    await context.SaveChangesAsync();
                }

    }
}
