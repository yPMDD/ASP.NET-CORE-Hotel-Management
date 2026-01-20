namespace HotelAurelia.Models
{
    public class ProgrammeFidelite
    {
        public int Id { get; set; }

        public int SeuilPoints { get; set; }

        public double ReductionPourcentage { get; set; }

        public ICollection<Client> Clients { get; set; } = new List<Client>();

        public decimal AppliquerReduction(Client client, decimal montant)
        {
            if (client.PointsFidelite >= SeuilPoints)
            {
                var reduction = (decimal)ReductionPourcentage / 100m * montant;
                return montant - reduction;
            }

            return montant;
        }
    }
}
