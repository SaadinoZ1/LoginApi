namespace LoginApi.Models
{
    public class Vehicule
    {
        public int Id { get; set; }
        public string? Modèle { get; set; }
        public string? PlaqueImmatriculation { get; set; }
        public ICollection<Trajet>? Trajets { get; set; }

    }
}
