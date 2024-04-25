using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginApiApp.Dtos
{
    public class RapportDto
    {
        public int Id { get; set; }
        public string? Evaluation { get; set; }
        public string? Commentaires { get; set; }
        public int TrajetId { get; set; }

    }
}
