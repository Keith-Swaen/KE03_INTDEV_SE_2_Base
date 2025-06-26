using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    // Interface voor onderdelen: ophalen, toevoegen, aanpassen en verwijderen.
    // IEnumerable betekent dat je een verzameling krijgt waar je overheen kunt lopen met foreach, zoals een lijst of een array.
    public interface IPartRepository
    {
        public IEnumerable<Part> GetAllParts();

        public Part? GetPartById(int id);

        public void AddPart(Part part);

        public void UpdatePart(Part part);

        public void DeletePart(Part part);
    }
}
