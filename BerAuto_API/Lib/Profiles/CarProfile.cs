namespace BerAuto_API.Lib.Profiles
{
    public class CarProfile : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Car, CarViewDTO>()
                .Map(dest => dest.CategoryName, src => src.Category.Name)
                .Map(dest => dest.DailyRate, src => src.Category.DailyRate);
        }
    }
}
