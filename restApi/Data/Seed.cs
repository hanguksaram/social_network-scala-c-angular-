using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace API.Data
{
    public class Seed
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        public Seed(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            this._roleManager = roleManager;
            this._userManager = userManager;
        }
        public void SeedUsers()
        {

            if (!_userManager.Users.Any())
            {
                var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
                var users = JsonConvert.DeserializeObject<List<User>>(userData);

                var roles = new List<Role>
                {
                    new Role{Name = Role.RoleTypes.Member.ToString()},
                    new Role{Name = Role.RoleTypes.Admin.ToString()},
                    new Role{Name = Role.RoleTypes.Moderator.ToString()},
                    new Role{Name = Role.RoleTypes.VIP.ToString()},
                  
                };

                foreach (var role in roles)
                {
                    _roleManager.CreateAsync(role).Wait();
                }
                foreach (var user in users)
                {
                    _userManager.CreateAsync(user, "password").Wait();
                    _userManager.AddToRoleAsync(user, Role.RoleTypes.Member.ToString()).Wait();
                }
                var adminUser = new User{
                    UserName = "Admin"
                };
                IdentityResult result = _userManager.CreateAsync(adminUser, "password").Result;

                if (result.Succeeded) {
                    var admin = _userManager.FindByNameAsync("Admin").Result;
                    _userManager.AddToRolesAsync(admin, new[] {Role.RoleTypes.Admin.ToString(), 
                        Role.RoleTypes.Moderator.ToString()}).Wait();
                }
            }

        }
        private (byte[] passwordSalt, byte[] passwordHash) CreatePasswordHash(string password)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {

                var passwordSalt = hmac.Key;
                var passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                return (passwordSalt, passwordHash);
            }

        }
    }
}