using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginApiApp.Models
{
    public class Vehicule
    {
        public int Id { get; set; }
        public string? Modèle { get; set; }
        public string? PlaqueImmatriculation { get; set; }
        public ICollection<Trajet>? Trajets { get; set; }

    }
}
