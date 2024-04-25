using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginApiApp.Dtos
{
    public class VehiculeDto
    {
        public int Id { get; set; }
        public string? Modèle { get; set; }
        public string? PlaqueImmatriculation { get; set; }
    }
}
