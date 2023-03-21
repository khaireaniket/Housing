using Housing.Domain.Entities.Base;

namespace Housing.Domain.Entities
{
    public class House : EntityBase
    {
        public House(string houseId, string adres, string postcode, string woonplaats,
                     string verkoopStatus, int makelaarId, string makelaarNaam, bool hasTuin)
        {
            HouseId = houseId;
            Adres = adres;
            Postcode = postcode;
            Woonplaats = woonplaats;
            VerkoopStatus = verkoopStatus;
            MakelaarId = makelaarId;
            MakelaarNaam = makelaarNaam;
            HasTuin = hasTuin;
        }

        public string HouseId { get; set; }

        public string Adres { get; set; } = default!;

        public string Postcode { get; set; } = default!;

        public string Woonplaats { get; set; } = default!;

        public string VerkoopStatus { get; set; } = default!;

        public int MakelaarId { get; set; }

        public string MakelaarNaam { get; set; } = default!;

        public bool HasTuin { get; set; }
    }
}
