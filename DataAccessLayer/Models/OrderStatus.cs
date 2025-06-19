using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    
    public enum OrderStatus
    {
        InWacht = 0,
        InBehandeling = 1,
        Verzonden = 2,
        Afgeleverd = 3,
        Geannuleerd = 4,
        Retour = 5
    }

    
}
