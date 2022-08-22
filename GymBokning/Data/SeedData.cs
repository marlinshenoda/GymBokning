using GymBokning.Models.Entity;
using Microsoft.AspNetCore.Identity;

namespace GymBokning.Data
{
    public class SeedData
    {
        private static ApplicationDbContext db;
        private static RoleManager<IdentityRole> roleManager=default!;
        private static UserManager<ApplicationUser> userManager= default!;

        public static async Task InitAsync(ApplicationDbContext context, IServiceProvider services)//, string adminPW)
        {
            //if (string.IsNullOrWhiteSpace(adminPW))
            //{
            //    throw new Exception("Cant get password from config");
            //}

            if (context is null) throw new NullReferenceException(nameof(ApplicationDbContext));

            db = context;

            if (db.GymClasses.Any()) return;

            roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            if (roleManager is null) throw new NullReferenceException(nameof(RoleManager<IdentityRole>));

            userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            if (userManager is null) throw new NullReferenceException(nameof(UserManager<ApplicationUser>));

            var roleNames = new[] { "Instructors", "Members", "Admin" };
            string adminEmail = "admin@gymbokinig.se";
            string adminPW = "Bytmig21";

            var gymclasses = GetGymClasses();
            await db.AddRangeAsync(gymclasses);

            await AddRolesAsync(roleNames);

            var admin = await AddAdminAsync(adminEmail, adminPW);
            await AddToRolesAsync(admin, roleNames);

            await db.SaveChangesAsync();


        }

        private static IEnumerable<GymClass> GetGymClasses()
        {
            var gymClasses = new List<GymClass>();
            var gymClassNames = new[] { "Yoga For All",
                                        "Morning Yoga",
                                        "Pilates",
                                        "Power Hour",
                                        "Fab Abs",
                                        "Zumba",
                                        "Kettlebell Circuits",
                                         "Morning Yoga",
                                         "Love Pilates"
            };
            int num = -10;
            foreach (var item in gymClassNames)
            {
                var temp = new GymClass
                {
                    Name = item,
                    Description = item + " for healthy body",
                    Duration = new TimeSpan(1, 15, 0),
                    StartTime = DateTime.Now.AddDays(num)
                };
                gymClasses.Add(temp);
                num += 5;
            }




            return gymClasses;
        }

        private static async Task AddToRolesAsync(ApplicationUser admin, string[] roleNames)
        {
            if (admin is null) throw new NullReferenceException(nameof(admin));

            foreach (var role in roleNames)
            {
                if (await userManager.IsInRoleAsync(admin, role)) continue;
                var result = await userManager.AddToRoleAsync(admin, role);
                if (!result.Succeeded) throw new Exception(string.Join("\n", result.Errors));
            }
        }

        private static async Task<ApplicationUser> AddAdminAsync(string adminEmail, string adminPW)
        {
            var found = await userManager.FindByEmailAsync(adminEmail);

            if (found != null) return null;

            var admin = new ApplicationUser
            {
                FirstName = "John",
                LastName = "Johansson",
                TimeOfRegistration = DateTime.Now,
                UserName = adminEmail,
                Email = adminEmail
            };

            var result = await userManager.CreateAsync(admin, adminPW);
            if (!result.Succeeded) throw new Exception(string.Join("\n", result.Errors));

            return admin;
        }

        private static async Task AddRolesAsync(string[] roleNames)
        {
            foreach (var roleName in roleNames)
            {
                if (await roleManager.RoleExistsAsync(roleName)) continue;
                var role = new IdentityRole { Name = roleName };
                var result = await roleManager.CreateAsync(role);

                if (!result.Succeeded) throw new Exception(string.Join("\n", result.Errors));
            }
        }
    
    }
}
