using System.Collections.Generic;

namespace KE03_INTDEV_SE_2_Base.Models
{
    /*
     * ViewModel voor de homepagina.
     * Bevat het dashboardmodel zodat statistieken op de homepage getoond kunnen worden.
     */
    public class HomeViewModel
    {
        // Dashboardgegevens voor de homepage
        public DashboardViewModel Dashboard { get; set; }

        public HomeViewModel()
        {
            Dashboard = new DashboardViewModel();
        }
    }
}
