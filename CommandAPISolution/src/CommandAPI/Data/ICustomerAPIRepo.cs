using System.Collections.Generic;
using CustomerAPI.Models;

namespace CustomerAPI.Data
{
    public interface ICustomerAPIRepo
    {
        bool SaveChanges();
        IEnumerable<Customer> GetAllCustomers();
        Customer GetCustomerById(int id);
        void addSomeCustomers();
    }
}