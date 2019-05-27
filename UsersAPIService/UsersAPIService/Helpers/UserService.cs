using System;
using System.Collections.Generic;
using System.Linq;
using UsersAPIService.Models;

namespace UsersAPIService.Helpers
{
    public interface IUserService
    {
        Users Authenticate(string username, string password);
        IEnumerable<Users> GetAll();
        Users GetById(Guid id);
        Users Create(Users User, string Password);
        void Update(Users User, string password = null);
        void Delete(Guid id);
    }

    public class UserService : IUserService
    {
        private UsersContext _context;

        public UserService(UsersContext context)
        {
            _context = context;
        }

        public Users Authenticate(string Email, string Password)
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
                return null;

            var user = _context.Users.SingleOrDefault(x => x.Email == Email);

            // check if username exists
            if (user == null)
                return null;

            // check if password is correct
            if (!VerifyPasswordHash(Password, user.PasswordHash, user.PasswordSalt))
                return null;

            // authentication successful
            return user;
        }

        public IEnumerable<Users> GetAll()
        {
            return _context.Users;
        }

        public Users GetById(Guid id)
        {
            return _context.Users.Find(id);
        }

        public Users Create(Users user, string password)
        {
            // validation
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Password is required");

            if (_context.Users.Any(x => x.Email == user.Email))
            {
                throw new AppException("Email \"" + user.Email + "\" is already registered");
            }

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.CreatedAt = DateTime.Today;

            _context.Users.Add(user);
            _context.SaveChanges();

            return user;
        }

        public void Update(Users userParam, string password = null)
        {
            var user = _context.Users.Find(userParam.Userid);

            if (user == null)
                throw new AppException("User not found");

            if (userParam.Email != user.Email)
            {
                // username has changed so check if the new username is already taken
                if (_context.Users.Any(x => x.Email == userParam.Email))
                    throw new AppException("Email \"" + userParam.Email + "\" is already registered");
            }

            // update user properties
            user.Name = userParam.Name;
            user.Surname = userParam.Surname;
            user.Email = userParam.Email;

            // update password if it was entered
            if (!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void Delete(Guid id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }

        // private helper methods

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }
    }
}
