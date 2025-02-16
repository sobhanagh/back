namespace GamaEdtech.Backend.Common.Service.Factory
{
    using GamaEdtech.Backend.Common.Data.Enumeration;

    public interface IProvider<T>
        where T : Enumeration<byte>
    {
        T ProviderType { get; }
    }
}
