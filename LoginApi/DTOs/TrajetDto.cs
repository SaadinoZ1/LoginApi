namespace LoginApi.DTOs
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
