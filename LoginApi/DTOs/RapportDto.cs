namespace LoginApi.DTOs
{
    public class RapportDto
    {
        public int Id { get; set; }
        public string? Evaluation { get; set; }
        public string? Commentaires { get; set; }
        public int TrajetId { get; set; }

    }
}
