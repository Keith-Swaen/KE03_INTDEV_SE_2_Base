using Microsoft.AspNetCore.Mvc.Rendering;

namespace KE03_INTDEV_SE_2_Base.Models
{
    /*
     * ViewModel voor het aanmaken van een nieuwe bestelling.
     * Bevat informatie over de geselecteerde klant, producten, datum en keuzelijsten.
     */
    public class OrderCreateViewModel
    {
        // Id van de geselecteerde klant
        public int CustomerId { get; set; }
        // Lijst van producten die aan de bestelling worden toegevoegd
        public List<OrderProductViewModel> OrderProducts { get; set; } = new();
        // Datum waarop de bestelling wordt geplaatst
        public DateTime OrderDate { get; set; } = DateTime.Now;
        // Keuzelijst met alle klanten voor de dropdown
        public IEnumerable<SelectListItem> Customers { get; set; } = new List<SelectListItem>();
        // Keuzelijst met alle producten voor de dropdown
        public IEnumerable<SelectListItem> Products { get; set; } = new List<SelectListItem>();
    }

    /*
     * ViewModel voor een productregel binnen een bestelling.
     * Wordt gebruikt om per product het aantal vast te leggen.
     */
    public class OrderProductViewModel
    {
        // Id van het geselecteerde product
        public int ProductId { get; set; }
        // Hoeveelheid van het product in de bestelling
        public int Quantity { get; set; }
    }
}
