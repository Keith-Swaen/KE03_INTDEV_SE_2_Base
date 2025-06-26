using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    // Interface voor bestellingen: ophalen, toevoegen, aanpassen en verwijderen.
    // IEnumerable betekent dat je een verzameling krijgt waar je overheen kunt lopen met foreach, zoals een lijst of een array.
    public interface IOrderRepository
    {
        public IEnumerable<Order> GetAllOrders();

        public Order? GetOrderById(int id);

        public void AddOrder(Order order);

        public void UpdateOrder(Order order);

        public void DeleteOrder(Order order);
    }
}
