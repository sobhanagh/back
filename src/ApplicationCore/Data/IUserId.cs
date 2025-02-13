#pragma warning disable IDE0040 // Add accessibility modifiers
namespace GamaEdtech.Backend.Data
{
    public interface IUserId<TKey>
    where TKey : IEquatable<TKey>
    {
        TKey UserId { get; set; }
    }
}
#pragma warning restore IDE0040 // Add accessibility modifiers
