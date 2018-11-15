using System;
using System.Threading.Tasks;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;

        public AuthRepository(DataContext context)
        {
            this._context = context;
        }

        public async  Task<User> Login(string username, string password)
        {
            var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(x => x.UserName == username);
            if (user == null)
                return null;
            // if (!await Task.Run(() => VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt)))
            //     return null;
            return user;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using( var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt)) {
                
                
                var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computeHash.Length; i++)
                {
                    if (computeHash[i] != passwordHash[i]) return false;
                }
                return true;
            }
        }

        public async Task<User> Register(User user, string password)
        {
            
            var (passwordSalt, passwordHash) = await Task.Run(() => CreatePasswordHash(password));
            

            // user.PasswordHash = passwordHash;
            // user.PasswordSalt = passwordSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private (byte[] passwordSalt, byte[] passwordHash) CreatePasswordHash(string password)
        {
            using( var hmac = new System.Security.Cryptography.HMACSHA512()) {
                
                var passwordSalt = hmac.Key;
                var passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                
                return (passwordSalt, passwordHash);
            }
            
        }

        public async Task<bool> UserExists(string username)
        {
            if (await _context.Users.AnyAsync(x => x.UserName == username))
                return true;
                
            return false;
        }
    }
}