using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginApiApp.Dtos
{
    public class TrajetDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Kilometrage { get; set; }
        public int Duree { get; set; }
        public string? Gains { get; set; }
        public int VehiculeId { get; set; }
    }
}
