namespace GamaEdtech.Backend.Common.Mapping
{
    public class Mapper : MapsterMapper.Mapper, IMapper
    {
        public Mapper()
            : base()
        {
        }

        public Mapper(TypeAdapterConfig config)
            : base(config)
        {
        }
    }
}
