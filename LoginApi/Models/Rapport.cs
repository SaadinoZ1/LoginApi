namespace LoginApi.Models
{
    public class Rapport
    {
        public int Id { get; set; }
        public string? Evaluation { get; set; }
        public string? Commentaires { get; set; }
        public int TrajetId { get; set; }
        public Trajet? Trajet { get; set; }


    }
}
