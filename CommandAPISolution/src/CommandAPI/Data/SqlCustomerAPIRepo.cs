using System.Collections.Generic;
using CustomerAPI.Models;
using System.Linq;
using System.Threading;
using System;

namespace CustomerAPI.Data
{
    public class SqlCustomerAPIRepo : ICustomerAPIRepo
    {

        private readonly CustomerContext _context;
        public SqlCustomerAPIRepo(CustomerContext context)
        {
            _context = context;
        }

        public IEnumerable<Customer> GetAllCustomers()
        {
            return _context.CustomerItems.ToList();
        }
        public Customer GetCustomerById(int id)
        {
            Thread.Sleep(100);
            
            return _context.CustomerItems.FirstOrDefault(p => p.Id == id);
        }

        public void addSomeCustomers()
        {
            var maxId = 1; //_context.CustomerItems.Max(cust => cust.Id);

            for (int i = 1 ; i <= 1000 ; i++)
            {
                var cust = new Customer()
                {
                    Id = maxId + i,
                    Name = "Super customer " + i,
                    BankAccount = "1234568" + i,
                    Age = 30
                };

                _context.Add(cust);
            }

            _context.SaveChanges();
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}