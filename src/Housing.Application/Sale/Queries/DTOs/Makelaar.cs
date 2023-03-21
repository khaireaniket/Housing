namespace Housing.Application.Sale.Queries.DTOs
{
    public class Makelaar
    {
        public int MakelaarId { get; set; }

        public string MakelaarNaam { get; set; } = default!;

        public int NumberOfListedHouses { get; set; }
    }
}
