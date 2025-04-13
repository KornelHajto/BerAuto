namespace BerAuto_API.Lib.Profiles
{
    public class CategoryProfile : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Category, CategoryViewDTO>();
        }
    }

}