namespace GamaEdtech.Backend.Common.Mapping
{
    using System;

    public sealed class AdaptTwoWaysAttribute : Mapster.AdaptTwoWaysAttribute
    {
        public AdaptTwoWaysAttribute(Type type)
            : base(type)
        {
        }

        public AdaptTwoWaysAttribute(string name)
            : base(name)
        {
        }
    }
}
