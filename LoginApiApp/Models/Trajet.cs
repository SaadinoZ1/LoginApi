using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginApiApp.Models
{
    public class Trajet
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Kilometrage { get; set; }
        public int Duree { get; set; }
        public string? Gains { get; set; }
        public int VehiculeId { get; set; }
        public Vehicule? Vehicule { get; set; }
        public ICollection<Rapport>? Rapports { get; set; }
    }
}
