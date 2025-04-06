namespace BerAuto.DTO
{

    public class RentViewDTO
    {
        public Guid ID { get; set; }
        public Guid RenterID { get; set; }
        public string RenterName { get; set; }
        public ERentStatus Status { get; set; }
        public DateTime ApplicationTime { get; set; }
        public int Owed { get; set; }
    }

}