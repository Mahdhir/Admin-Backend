using System.Collections.Generic;
using Project.Entities;

namespace Project.Services
{
    public interface iAdminServices
    {
        Admin AuthenticateUser(string username, string password);
        
         IEnumerable<Admin> GetAllAdmins();
        Admin GetById(int id);
          Admin AddAdmin(Admin admin, string password);
         void UpdateAdmin(Admin admin, string password = null);

         void DeleteAdmin(int id);
    }
}