using BerAuto.Lib.Models;
using BerAuto.Lib.DTO;
using Mapster;

namespace BerAuto_API.Lib.Profiles
{
    public class CarProfile : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Car, CarViewDTO>()
                .Map(dest => dest.ID, src => src.ID)
                .Map(dest => dest.PlateNumber, src => src.PlateNumber)
                .Map(dest => dest.Type, src => src.Type)
                .Map(dest => dest.Odometer, src => src.Odometer)
                .Map(dest => dest.Available, src => src.Available)
                .Map(dest => dest.Description, src => src.Description)
                .Map(dest => dest.CategoryName, src => src.Category != null ? src.Category.Name : null)
                .Map(dest => dest.DailyRate, src => src.Category != null ? src.Category.DailyRate : 0);
        }
    }
}
