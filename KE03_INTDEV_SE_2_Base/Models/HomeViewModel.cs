using System.Collections.Generic;

namespace KE03_INTDEV_SE_2_Base.Models
{
    public class HomeViewModel
    {
        public DashboardViewModel Dashboard { get; set; }

        public HomeViewModel()
        {
            Dashboard = new DashboardViewModel();
        }
    }
}
