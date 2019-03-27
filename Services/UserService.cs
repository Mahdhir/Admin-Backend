using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebApi.Dtos;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Services;

namespace Project.Services
{
    public class UserService : IUserService
    {
         private DataContext _context;

        public UserService(DataContext context)
        {
            _context = context;
        }

        public User AuthenticateUser(string username, string password, bool IsSocialMedia)
        {
            if (!IsSocialMedia)
            {

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                    return null;

                var user = _context.Users.FirstOrDefault(x => x.Username == username);

                // check if username exists
                if (user == null)
                    return null;

                // check if password is correct
                if (!Encoder.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                    return null;

                // authentication successful
                return user;

            }
            else
            {

                if (string.IsNullOrEmpty(username))
                    return null;

                var user = _context.Users.FirstOrDefault(x => x.Username == username);

                // check if username exists
                if (user == null)
                {
                  return null;

                }

                // authentication successful
                return user;
            }

        }

        public IEnumerable<User> GetAllUser()
        {
            return _context.Users;
        }

        public User GetById(int id)
        {
            return _context.Users.Find(id);
        }

        public User RegisterUser(User user, string password)
        {
            if (!user.IsSocialMedia)
            {
                // validation
                if (string.IsNullOrWhiteSpace(password))
                    throw new AppException("Password is required");

                if (_context.Users.Any(x => x.Username == user.Username))
                    throw new AppException("Email \"" + user.Username + "\" is already taken");

                byte[] passwordHash, passwordSalt;
                Encoder.CreatePasswordHash(password, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;

                _context.Users.Add(user);
                _context.SaveChanges();

                return user;
            }
            else
            {

                if (_context.Users.Any(x => x.Username == user.Username))
                    throw new AppException("Email \"" + user.Username + "\" is already taken");

                _context.Users.AddAsync(user);
                _context.SaveChangesAsync();

                return user;
            }

        }

        public void UpdateUser(User userParam, string password = null)
        {
            var user = _context.Users.Find(userParam.Id);

            if (user == null)
                throw new AppException("User not found");

            if (userParam.Username != user.Username)
            {
                // username has changed so check if the new username is already taken
                if (_context.Users.Any(x => x.Username == userParam.Username))
                    throw new AppException("Email " + userParam.Username + " is already taken");
            }

            // update user properties
            user.FirstName = userParam.FirstName;
            user.LastName = userParam.LastName;
            user.Username = userParam.Username;

            // update password if it was entered
            if (!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordHash, passwordSalt;
                Encoder.CreatePasswordHash(password, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void DeleteUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }

        // private helper methods

        

       

        
    }
}