// BerAuto_Lib/DTO/RentalStatusUpdateDTO.cs
namespace BerAuto.DTO
{
    public class RentalStatusUpdateDTO
    {
        public string RentalId { get; set; }
        public ERentStatus NewStatus { get; set; }
    }
}