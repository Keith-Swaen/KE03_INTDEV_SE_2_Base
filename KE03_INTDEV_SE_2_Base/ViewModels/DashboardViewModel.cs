using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KE03_INTDEV_SE_2_Base.Models
{
    /*
     * ViewModel voor het dashboardoverzicht.
     * Bevat totalen, grafiekdata en top-producten voor de statistieken.
     */
    public class DashboardViewModel
    {
        // Totaal aantal producten in het systeem
        public int TotalProducts { get; set; }
        // Totaal aantal klanten
        public int TotalCustomers { get; set; }
        // Totaal aantal bestellingen
        public int TotalOrders { get; set; }
        // Totale omzet (in euro's)
        public decimal TotalRevenue { get; set; }
        // Data voor de prijscategorie-verdeling van producten
        public List<ChartDataPoint> ProductPriceDistribution { get; set; }
        // Data voor het aantal bestellingen per dag
        public List<ChartDataPoint> OrdersOverTime { get; set; }
        // Lijst van best verkochte producten
        public List<TopProductViewModel> TopProducts { get; set; }

        public DashboardViewModel()
        {
            ProductPriceDistribution = new List<ChartDataPoint>();
            OrdersOverTime = new List<ChartDataPoint>();
            TopProducts = new List<TopProductViewModel>();
        }
    }

    /*
     * Dataobject voor een punt in een grafiek (label + waarde).
     */
    public class ChartDataPoint
    {
        [Required]
        // Naam van de categorie of dag
        public string Label { get; set; } = string.Empty;
        // Waarde die bij het label hoort
        public double Value { get; set; }
    }

    /*
     * ViewModel voor een top-product in het dashboard.
     */
    public class TopProductViewModel
    {
        [Required]
        // Naam van het product
        public string ProductName { get; set; } = string.Empty;
        // Hoe vaak dit product is besteld
        public int OrderCount { get; set; }
        // Totale omzet van dit product
        public decimal TotalRevenue { get; set; }
    }
} 