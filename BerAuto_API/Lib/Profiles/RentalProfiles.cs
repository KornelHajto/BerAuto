namespace BerAuto_API.Lib.Profiles
{
    public class RentalProfiles : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<(Rent rent, string renterName), RentViewDTO>()
                .Map(dest => dest.ID, src => src.rent.ID)
                .Map(dest => dest.RenterID, src => src.rent.RenterID)
                .Map(dest => dest.RenterName, src => src.renterName)
                .Map(dest => dest.Status, src => src.rent.Status)
                .Map(dest => dest.ApplicationTime, src => src.rent.ApplicationTime)
                .Map(dest => dest.Owed, src => src.rent.Owed);
        }
    }
}