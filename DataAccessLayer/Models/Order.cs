﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    // Model voor een bestelling, bevat info over klant, datum, status en producten.
    public class Order
    {
        public int Id { get; set; }

        public DateTime OrderDate { get; set; }

        public int CustomerId { get; set; }
        
        public Customer Customer { get; set; } = null!;

        public OrderStatus Status { get; set;}

        public ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();

    }
}
