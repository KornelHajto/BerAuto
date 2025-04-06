// BerAuto_Lib/DTO/RentalCreateDTO.cs
namespace BerAuto.DTO
{
    public class RentalCreateDTO
    {
        public Guid RenterId { get; set; }
        public Guid CarId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}