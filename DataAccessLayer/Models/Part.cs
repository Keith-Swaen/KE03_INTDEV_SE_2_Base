using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    // Model voor een onderdeel, bevat info zoals naam, omschrijving en voorraad.
    public class Part
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public virtual ICollection<Product> Products { get; } = new List<Product>();
    }
}
