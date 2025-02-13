namespace GamaEdtech.Backend.Data
{
#pragma warning disable IDE0040 // Add accessibility modifiers
    public interface IUserId<TKey>
#pragma warning restore IDE0040 // Add accessibility modifiers
    where TKey : IEquatable<TKey>
    {
        TKey UserId { get; set; }
    }
}
