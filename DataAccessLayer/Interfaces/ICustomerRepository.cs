using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    // Interface voor klanten: ophalen, toevoegen, aanpassen en verwijderen.
    // IEnumerable betekent dat je een verzameling krijgt waar je overheen kunt lopen met foreach, zoals een lijst of een array.
    public interface ICustomerRepository
    {
        public IEnumerable<Customer> GetAllCustomers();

        public Customer? GetCustomerById(int id);

        public void AddCustomer(Customer customer);

        public void UpdateCustomer(Customer customer);

        public void DeleteCustomer(Customer customer);
    }
}
