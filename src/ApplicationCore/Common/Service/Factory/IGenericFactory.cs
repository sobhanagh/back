namespace GamaEdtech.Backend.Common.Service.Factory
{
    using GamaEdtech.Backend.Common.Data.Enumeration;

    [DataAnnotation.Injectable]
    public interface IGenericFactory<TProvider, TProviderType>
        where TProvider : IProvider<TProviderType>
        where TProviderType : Enumeration<byte>
    {
        TProvider? GetProvider(TProviderType providerType, bool returnFirstItemIfNotMatch = false);
    }
}
