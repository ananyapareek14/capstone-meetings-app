using Microsoft.EntityFrameworkCore;
using meetings_app_server.Models.Domain;
using System.Security.Cryptography;
using System.Text;
using meetings_app_server.Data;

namespace meetings_app_server.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Register user
        public async Task<User> Register(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }


        // Login user by verifying email and password
        public async Task<User?> Login(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return null; // User not found
            }

            // Verify password hash
            if (VerifyPassword(password, user.PasswordHash))
            {
                return user; // Login successful
            }

            return null; // Incorrect password
        }

        // Save session key for a user
        public async Task SaveUserSessionKey(Guid userId, string sessionKey)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.SessionKey = sessionKey;
                await _context.SaveChangesAsync();
            }
        }

        // Retrieve session key for a user
        public async Task<string?> GetUserSessionKey(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            return user?.SessionKey;
        }

        // Verify password (simple example, should be more secure in production)
        private bool VerifyPassword(string plainPassword, string hashedPassword)
        {
            return hashedPassword == HashPassword(plainPassword);
        }

        // Hash password (simple example, should be more secure in production)
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
