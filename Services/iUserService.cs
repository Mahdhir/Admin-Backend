using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Dtos;
using WebApi.Entities;
using WebApi.Helpers;

namespace WebApi.Services
{
    public interface IUserService
    {
         User AuthenticateUser(string username, string password,bool IsSocialMedia);
        IEnumerable<User> GetAllUser();
        User GetById(int id);
         User RegisterUser(User user, string password);
        void UpdateUser(User user, string password = null);
        void DeleteUser(int id);
    }

    // public class UserService 
    // {
        
    // }
}