namespace GamaEdtech.Backend.Common.Mvc.Routing
{
    using GamaEdtech.Backend.Common.Core;

    using Microsoft.AspNetCore.Routing;

    public class SlugifyParameterTransformer : IOutboundParameterTransformer
    {
        public string? TransformOutbound(object? value) => (value as string).Slugify();
    }
}
