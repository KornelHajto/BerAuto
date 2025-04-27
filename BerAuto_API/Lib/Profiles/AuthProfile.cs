
namespace BerAuto_API.Lib.Profiles
{
    public class AuthProfile : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<RegisterDTO, User>();
            config.NewConfig<User, LoginResponseDTO>();
        }
    }
}