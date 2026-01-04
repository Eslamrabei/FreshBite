namespace Service.MappingProfiles
{
    public class RefreshTokenMapping : Profile
    {
        public RefreshTokenMapping()
        {

            CreateMap<RefreshToken, RefreshTokenDto>()
                .ForMember(dest => dest.User, opt => opt.Ignore());


            CreateMap<RefreshTokenDto, RefreshToken>()
                .ForMember(dest => dest.User, opt => opt.Ignore());

        }
    }
}
